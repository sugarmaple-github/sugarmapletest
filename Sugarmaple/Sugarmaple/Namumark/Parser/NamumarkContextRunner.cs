using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Text;
using Sugarmaple.Namumark.Parser.Keywords;
using Sugarmaple.Namumark.Parser.Tokens;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamumarkContextRunner
  {
    private readonly NamumarkRegContext _regContext;
    private readonly string _source;
    private readonly List<StringRange> _tokenBuffer = new List<StringRange>();
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

    public NamumarkContextRunner(NamumarkRegContext matcher, string source): this(matcher, source, 0, source.Length) {}
    
    public NamumarkContextRunner(NamumarkRegContext matcher, string source, int start, int end, OpenTokenCommand? lifoOpen = null)
    {
      _regContext = matcher;
      _source = source;
      _pos = _start = start;
      _end = end;
      _lifoOpen = lifoOpen;
    }

    public IEnumerable<NamuToken> GetTokens()
    {
      NamuToken? token;
      do
      {
        token = GetToken();
        if (token == null) yield break;
        yield return token;
      } while (true);
    }
    
    #region Private Methods
    //null if End
    //Iterate until getToken or end
    private NamuToken? GetToken()
    {
      var token = ProcessMatch();
      while (token is not NamuToken)
      {
        token = ProcessMatch();
        if (token is InnerToken inner && inner.Operation == Operation.End)
          return null;
      }
      return (NamuToken)token;
    }

    //Process Single Match
    //Return Match Token
    private StringRange ProcessMatch()
    {
      _match = _regContext.Match(_source, _pos, _end - _pos);
      Console.WriteLine($"Processd Match Null Checking: {_match == null}");
      if (_match == null)
        return CreateInnerToken(Operation.End);
      
      var result = ExecuteCommands();
      _pos = result != null ? result!.End : Match.End;
      Console.WriteLine(_pos);
      if(!BufferEmpty && result is NamuToken element)
      {
        AddBuffer(element);
        return CreateInnerToken(Operation.None);
      }
      //나올 수 있는 토큰들
      //NamuToken : 완전 성공
      //Token.Fail : lifo 실패함! (서브 토크나이저 전용)
      //Token.Close : 닫는 구문 받음. (서브 토크나이저 전용)
      //null : 이스케이프 구문이었거나, buffer에 값을 넣었음, close 혹은 fail이지만 유의미한 구문이 없어서 포기함.
      if(result is InnerToken token)
      {
        switch (token.Operation)
        {
          case Operation.Close:
            _pos = token.End;
            break;
          case Operation.Fail:
            _pos++;
            break;
        }
      }
      return result ?? CreateInnerToken(Operation.None);
    }

    private StringRange ExecuteCommands()
    {
      IEnumerable<NamuToken>? children = null;
      foreach (var group in Match.Groups.Skip(1))
      {
        //if(group.Command.Type != CommandType.None)
        Console.WriteLine($"Child Type: {group.Command.Type}");
        var result = ExecuteCommand(group.Command, group);
        if (result is IEnumerable<NamuToken> newChildren)
        {
          children = newChildren;
          Console.WriteLine("chilren Detected");
        }
        //만약 구문 생성기라면, 어딘가에 저장할 것
      }

      foreach(var command in Match.OverrideCommands.Prepend(Match.Command))
      {
        var result = (StringRange?)ExecuteCommand(command, Match);
        if (result is NamuToken parent && children != null)
          parent.Children = children;
        if(result != null) return result;
      }
      return CreateInnerToken(Operation.None);
    }

    private object? ExecuteCommand(TokenCommand command, StringRange range)
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
            return CreateInnerToken(Operation.Close);
          break;
        case CommandType.Fail:
          if (_lifoOpen != null && _lifoOpen.IsFailPartner(command))
            return CreateInnerToken(Operation.Fail);
          break;
        case CommandType.AccumulateAsList:
          return ExecuteAccumulateList(command);
        case CommandType.AccumulateAsLine:
          break;
        case CommandType.Complex:
          throw new ArgumentException("command cant have Type.Complex in runner.");
        case CommandType.Context:
          return ExecuteContext(range);
      }
      return null;
    }

    private IEnumerable<NamuToken> ExecuteContext(StringRange range)
    {
      return GetSubstringTokens(range.Index, range.End);
    }

    //support only match
    private NamuToken ExecuteIntact(SyntaxCode code) => CreateTokenFromMatch(code);

    private StringRange ExecuteLifo(OpenTokenCommand command)
    {
      var tokens = GetTokensFromOther(command.Context ?? _regContext, command);
      var children = new List<NamuToken>();
      InnerToken? lastInner = null;
      
      foreach (var t in tokens)
      {
        if (t is InnerToken inner)
        {
          lastInner = inner;
          break;
        }
        children.Add((NamuToken)t);
      }
      if (lastInner == null || lastInner.Operation != Operation.Close)
        return CreateInnerToken(Operation.Fail);

      return CreateTokenUntilCloser(command.SyntaxCode, lastInner, children);
    }

    private NamuToken? ExecuteFifo(OpenTokenCommand command)
    {
      var first = PeekBuffer();
      if (first != null && first.Command.IsClosePartner(command))
        return CreateTokenFromBuffer(command.SyntaxCode);
      AddBuffer(command);
      return null;
    }

    private NamuToken ExecuteAccumulateList(TokenCommand command)
    {
      return null;
    }
    #endregion

    private void AddBuffer(NamuToken token)
    {
      _tokenBuffer.Add(token);
    }

    private void AddBuffer(OpenTokenCommand command)
    {
      _tokenBuffer.Add(new OpenToken(Match.Index, Match.Length, command));
    }

    private OpenToken? PeekBuffer() => BufferEmpty ? null : (OpenToken)_tokenBuffer[0];
      
    private IEnumerable<NamuToken> GetSubstringTokens(int start, int end) => (new NamumarkContextRunner(_regContext, _source, start, end)).GetInnerTokens().Cast<NamuToken>();

    private IEnumerable<StringRange> GetTokensFromOther(NamumarkRegContext context, OpenTokenCommand command) => (new NamumarkContextRunner(context, _source, Match.End, _source.Length, command)).GetInnerTokens();

    private IEnumerable<StringRange> GetInnerTokens()
    {
      var token = ProcessMatch();
      if(token == null) yield break;
      yield return token;
    }

    private InnerToken CreateInnerToken(Operation operation) => new InnerToken(Match.Index, Match.Length, operation);

    private NamuToken CreateTokenUntilCloser(SyntaxCode code, InnerToken closer, IEnumerable<NamuToken> children) =>
      new NamuToken(code, Match.Groups, Match.Index, closer.Index + closer.Length - Match.Index, children);
    
    private NamuToken CreateTokenFromMatch(SyntaxCode code, IEnumerable<NamuToken>? children = null) => new NamuToken(code, Match.Groups, Match.Index, Match.Length, children);
    
    private NamuToken CreateTokenFromBuffer(SyntaxCode code)
    {
      var index = _tokenBuffer[0].Index;
      var length = Match.End - index;
      var children = _tokenBuffer.OfType<NamuToken>();
      _tokenBuffer.Clear();
      return new NamuToken(code, Match.Groups, index, length, children);
    }

    //Indicate Inner Operations
    private class InnerToken: StringRange
    {
      public Operation Operation { get; }

      public InnerToken(int index, int length, Operation operation): base(index, length)
      {
        Operation = operation;
      }
    }

    /*private class ChildToken: StringRange
    {
      public IEnumerable<NamuToken> Value { get; }

      public InnerToken(int index, int length, IEnumerable<NamuToken> value): base(index, length)
      {
        Value = value;
      }
    }*/

    internal class OpenToken: StringRange
    {
      public OpenTokenCommand Command { get; }

      public OpenToken(int index, int length, OpenTokenCommand command): base(index, length)
      {
        Command = command;
      }
    }

    private enum Operation: byte
    {
      None,
      Close,
      Fail,
      End,
    }
  }
}