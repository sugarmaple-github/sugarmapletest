using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sugarmaple.Text;
using Sugarmaple.Namumark.Parser.Keywords;
using Sugarmaple.Namumark.Parser.Tokens;

namespace Sugarmaple.Namumark.Parser
{
  internal class Tokenizer
  {
    private readonly Context _context;
    private readonly string _source;
    private readonly List<Token> _tokenBuffer = new List<Token>();
    private readonly int _start;
    private readonly int _end;
    private readonly OpenTokenCommand? _lifoOpen = null;
    
    private int _pos;
    
    private PatternMatch? _match;
    private PatternMatch Match 
    {
      get => _match! ?? throw new InvalidOperationException("_match가 null일 때 호출되어서는 안됩니다.");
      set => _match = value;
    }
    private bool BufferEmpty => _tokenBuffer.Count == 0;

    //public bool MoveNext { get; private set; }
    public bool IsEnd { get; private set; }

    public Tokenizer(Context matcher, string source): this(matcher, source, 0, source.Length) {}
    
    public Tokenizer(Context matcher, string source, int start, int end, OpenTokenCommand? lifoOpen = null)
    {
      _context = matcher;
      _source = source;
      _pos = _start = start;
      _end = end;
      _lifoOpen = lifoOpen;
    }

    public ElementToken? GetToken()
    {
      return (ElementToken?)GetToken(true);
    }
    
    #region Private Methods
    //null if End
    private Token? GetToken(bool isElementOnly)
    {
      Token? token;
      do
      {
        token = ProcessMatch();
        if (isElementOnly && token is ElementToken)
          break;
        if (!isElementOnly && token != null)
          break;
        
      } while (!IsEnd);
      return token;
    }
      
    private Token? ProcessMatch()
    {
      _match = _context.Match(_source, _pos, _end - _pos);
      if (_match == null)
      {
        IsEnd = true;
        return null;
      }
      
      var token = ExecuteCommands();
      _pos = token != null ? token!.End : Match.End;
      Console.WriteLine(_pos);
      if(!BufferEmpty && token is ElementToken element)
      {
        AddBuffer(element);
        return null;
      }
      //나올 수 있는 토큰들
      //ElementToken : 완전 성공
      //Token.Fail : lifo 실패함! (서브 토크나이저 전용)
      //Token.Close : 닫는 구문 받음. (서브 토크나이저 전용)
      //null : 이스케이프 구문이었거나, buffer에 값을 넣었음, close 혹은 fail이지만 유의미한 구문이 없어서 포기함.
      if(token != null)
      {
        switch (token.Operation)
        {
          case TokenizeOperation.Close:
            _pos = token.End;
            return token;
          case TokenizeOperation.Fail:
            _pos++;
            return token;
        }
      }
      return token;
    }

    private Token? ExecuteCommands()
    {
      foreach(var command in Match.Commands)
      {
        var token = ExecuteCommand(command);
        if(token != null) return token;
      }
      return null;
    }

    private Token? ExecuteCommand(TokenCommand command)
    {
      switch (command.Type)
      {
        case CommandType.Intact: 
          return ExecuteIntact(command.SyntaxCode);
        case CommandType.OpenLifo:
          return ExecuteLifo((OpenTokenCommand)command);
        case CommandType.OpenCloseFifo:
          return ExecuteFifo((OpenTokenCommand)command);
        case CommandType.Close:
          if (_lifoOpen != null && _lifoOpen.IsClosePartner(command))
            return CreateToken(TokenizeOperation.Close);
          break;
        case CommandType.Fail:
          if (_lifoOpen != null && _lifoOpen.IsFailPartner(command))
            return CreateToken(TokenizeOperation.Fail);
          break;
        case CommandType.AccumulateAsList:
          return ExecuteAccumulateList(command);
          break;
        case CommandType.AccumulateAsLine:
          break;
      }
      return null;
    }

    private ElementToken ExecuteIntact(SyntaxCode code)
    {
      IEnumerable<ElementToken>? children = null;
      foreach (var g in Match.Groups)
      {
        if(g.IsWikiBlock)
        {
          children = GetSubstringTokens(g.Index, g.End);
          break;
        }
      }
      return CreateToken(code, children);
    }

    private Token ExecuteLifo(OpenTokenCommand command)
    {
      var tokens = GetTokensFromOther(command.Context ?? _context, command);
      var children = new List<ElementToken>();
      Token? last = null;
      
      foreach (var t in tokens)
      {
        last = t;
        if(t.Operation == TokenizeOperation.Fail || t.Operation == TokenizeOperation.Close)
          break;
        children.Add((ElementToken)t);
      }
      if (last == null || last.Operation != TokenizeOperation.Close)
        return CreateToken(TokenizeOperation.Fail);

      return CreateTokenUntilCloser(command.SyntaxCode, last, children);
    }

    private Token? ExecuteFifo(OpenTokenCommand command)
    {
      var first = PeekBuffer();
      if (first != null && first.Command.IsClosePartner(command))
        return CreateTokenFromBuffer(command.SyntaxCode);
      AddBuffer(command);
      return null;
    }

    private ElementToken ExecuteAccumulateList(TokenCommand command)
    {
      return null;
    }
    #endregion

    private void AddBuffer(ElementToken token)
    {
      _tokenBuffer.Add(token);
    }

    private void AddBuffer(OpenTokenCommand command)
    {
      _tokenBuffer.Add(new OpenToken(Match.Index, Match.Length, command));
    }

    private OpenToken? PeekBuffer() => BufferEmpty ? null : (OpenToken)_tokenBuffer[0];
      
    private IEnumerable<ElementToken> GetSubstringTokens(int start, int end) => (new Tokenizer(_context, _source, start, end)).GetTokens().Cast<ElementToken>();

    private IEnumerable<Token> GetTokensFromOther(Context context, OpenTokenCommand command) => (new Tokenizer(context, _source, Match.End, _source.Length, command)).GetTokens();

    private IEnumerable<Token> GetTokens()
    {
      while (!IsEnd)
      {
        var token = GetToken(false);
        yield return token!;
      }
    }

    private Token CreateToken(TokenizeOperation operation) => new Token(Match.Index, Match.Length, operation);

    private ElementToken CreateTokenUntilCloser(SyntaxCode code, Token closer, IEnumerable<ElementToken> children) =>
      new ElementToken(code, Match.Groups, Match.Index, closer.Index + closer.Length - Match.Index, children);
    
    private ElementToken CreateToken(SyntaxCode code, IEnumerable<ElementToken>? children) => new ElementToken(code, Match.Groups, Match.Index, Match.Length, children);
    
    private ElementToken CreateTokenFromBuffer(SyntaxCode code)
    {
      var index = _tokenBuffer[0].Index;
      var length = Match.End - index;
      var children = _tokenBuffer.OfType<ElementToken>();
      _tokenBuffer.Clear();
      return new ElementToken(code, Match.Groups, index, length, children);
    }
  }
}