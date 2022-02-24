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
    private readonly PatternInfo[] _patterns;
    private readonly Regex _regex;
    private readonly List<TokenCommand>[] _commands;
    private readonly List<int> _namedGroupIndice;

    public Tokenizer(params Keyword[] keywords)
    {
      _patterns = keywords.Select(o => o.Pattern).ToArray();  
      _regex = BuildRegex(keywords);
      _commands = BuildCommandSet(keywords);
      _namedGroupIndice = BuildNamedGroupIndice(keywords);
    }

    public IEnumerable<ElementToken> GetTokens(string source) => GetElementTokens(source, 0, source.Length);
    
    #region Private Methods
    private IEnumerable<ElementToken> GetElementTokens(string source, int startat, int length) => GetTokens(source, startat, length, null, true).Cast<ElementToken>();
    private IEnumerable<Token> GetTokens(string source, int startat, OpenTokenCommand? open) => GetTokens(source, startat, source.Length - startat, open);
    private IEnumerable<Token> GetTokens(string source, int startat, int length, OpenTokenCommand? open = null, bool getElementOnly = false)
    {
      int i = startat;
      var fifoOpenBuffer = new List<OpenTokenCommand>();
      var tokenBuffer = new List<Token>();
      while (i < source.Length)
      {
        var match = Match(source, i, length + i - startat);
        if (match == null)
          break;
        var token = ExecuteCommands(match!, tokenBuffer, open);
        switch (token.Operation)
        {
          case TokenizeOperation.Element:
            yield return token;
          case TokenizeOperation.Open:
            tokenBuffer.Add(token);
            
          case TokenizeOperation.Close:
            yield return token;
            yield break;
          case TokenizeOperation.OpenClose:
            yield return token;
    
          case TokenizeOperation.Fail:
            yield return token;
            yield break;
        }
        
        if(!getElementOnly || (getElementOnly && token is ElementToken element))
          yield return token;
        i = match.End;
      }
    }

    private Token? ExecuteCommands(PatternMatch match, List<Token> tokenBuffer, OpenTokenCommand? open)
    {
      var index = match.Index;
      foreach(var command in match.Commands)
      {
        var token = ExecuteCommand(command, match, tokenBuffer, open);
        if(token != null) return token;
      }
      return null;
    }

    private Token? ExecuteCommand(TokenCommand command, PatternMatch match, List<Token> tokenBuffer, OpenTokenCommand? open)
    {
      switch (command.Type)
      {
        case CommandType.Intact: 
          return ExecuteIntact(command.SyntaxCode, match);
        case CommandType.OpenLifo:
          return ExecuteLifo((OpenTokenCommand)command, match);
        case CommandType.OpenCloseFifo:
          return ExecuteFifo((OpenTokenCommand)command, match, tokenBuffer);
        case CommandType.Close:
          if (open != null && open.IsClosePartner(command))
            return new Token(match.Index, match.Length, TokenizeOperation.Close);
          break;
        case CommandType.Fail:
          if (open != null && open.IsFailPartner(command))
            return new Token(match.Index, match.Length, TokenizeOperation.Fail);
          break;
      }
      return null;
    }

    private ElementToken ExecuteIntact(SyntaxCode code, PatternMatch match)
    {
      IEnumerable<ElementToken>? children = null;
      foreach (var g in match.Groups)
      {
        if(g.IsWikiBlock)
        {
          children = GetElementTokens(match.Text, g.Index, g.Length);
          break;
        }
      }
      return new ElementToken(code, match.Groups, match.Index, match.Length, children);
    }

    private Token? ExecuteLifo(OpenTokenCommand command, PatternMatch match)
    {
      var context = command.Tokenizer ?? this;
      var tokens = context.GetTokens(match.Text, match.End, command);
      var children = new List<ElementToken>();
      Token? last = null;
      foreach (var t in tokens)
      {
        if(t.Operation == TokenizeOperation.Fail || t.Operation == TokenizeOperation.Close)
          break;
        children.Add((ElementToken)t);
        last = t;
      }
      if(last == null || last.Operation != TokenizeOperation.Close)
        return new Token(match.Index, match.Length, TokenizeOperation.Fail);

      return new ElementToken(command.SyntaxCode, match.Groups, match.Index, last.Index + last.Length - match.Index, children);
    }

    private Token? ExecuteFifo(OpenTokenCommand command, PatternMatch match, List<Token> tokenBuffer)
    {
      foreach(var t in tokenBuffer)
      {
        if (t.IsClosePartner(command))
        {
          var length = last.Index + last.Length - match.Index;
          return new ElementToken(command.SyntaxCode, match.Groups, match.Index, length, GetChildren()); //need to find
        }
      }
      fifoOpens.Add(command);
      return null;
      
      IEnumerable<ElementToken> GetChildren() => tokenBuffer.TypeOf<ElementToken>();
    }

    private PatternMatch? Match(string source, int startat, int length)
    {
      var m = _regex.Match(source, startat, length);
      for (int i = 0; i < _patterns.Length; i++)
      {
        if (m.Groups[_namedGroupIndice[i]].Success)
        {
          var pattern = _patterns[i];
          var groupIndex = 1;
          var argument = m.Groups.Cast<Group>().Skip(_namedGroupIndice[i]+1).Take(pattern.GroupNum-1).Select(o => new PatternGroup(source, o.Index, o.Length, groupIndex++ == pattern.MarkableGroup)).ToArray();

          return new PatternMatch(source, m.Index, m.Length, argument, _commands[i]);
        }
      }
      return null;
    }
    #endregion

    #region Constructor Helper Build Methods
    private static Regex BuildRegex(Keyword[] keywords)
    {
      var buffer = StringBuilderPool.Obtain();
      for (int i = 0; i < keywords.Length; i++)
      {
        var pattern = keywords[i].Pattern.Raw;
        buffer.Append('(').Append(pattern).Append(')').Append('|');
      }
      buffer.Length--;
      var regex = new Regex(buffer.ToString(), RegexOptions.Multiline | RegexOptions.Singleline);
      buffer.ToPool();
      return regex;
    }

    private static List<TokenCommand>[] BuildCommandSet(Keyword[] keywords)
    {
      var ret = new List<TokenCommand>[keywords.Length];
      for (int i = 0; i < ret.Length; i++)
      {
        ret[i] = new List<TokenCommand>();
        ret[i].Add(keywords[i].Command);
      }
      for (int i = 0; i < keywords.Length - 1; i++)
      {
        for (int j = i + 1; j < keywords.Length; j++)
        {
          var pti = keywords[i].Pattern.Raw;
          var ptj = keywords[j].Pattern.Raw;
          if(pti.Contains(ptj))
            ret[i].Add(ret[j][0]);
        }
      }
      //어떤 패턴이 다른 패턴을 포함할 때,
      //포함하는 패턴은 포함된 패턴의 기능을 상속받아야 한다.

      //+패턴을 나열할 때는 포함하는 패턴이 포함된 패턴보다 선행되어야 한다.
      return ret;
    }

    private static List<int> BuildNamedGroupIndice(Keyword[] keywords)
    {
      var ret = new List<int>();
      ret.Add(1);
      for (var i = 1; i < keywords.Length; i++)
      {
        var nextIndex = ret[i-1] + keywords[i-1].Pattern.GroupNum;
        ret.Add(nextIndex);
      }
      return ret;
    }
    #endregion
  }
}