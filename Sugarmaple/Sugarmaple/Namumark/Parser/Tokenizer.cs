using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sugarmaple.Text;
using Sugarmaple.Namumark.Parser.Keywords;

namespace Sugarmaple.Namumark.Parser
{
  internal class Tokenizer
  {
    private readonly PatternInfo[] _patterns;
    private readonly Regex _regex;
    private readonly List<MatchFollowUp>[] _followUps;
    //private readonly List<Tokenizer> _otherTokenizers;
    private readonly List<int> _namedGroupIndice;
    //private readonly Keyword? _triggerKeyword;

    /*public Tokenizer(params Keyword[] keywords): this(keywords, null, null)
    {
    }*/

    public Tokenizer(params Keyword[] keywords)// Tokenizer? inserted)
    {
      _patterns = keywords.Select(o => o.Pattern).ToArray();  
      _regex = BuildRegex(keywords);
      _followUps = BuildFollowUpSet(keywords);
      //_otherTokenizers = BuildOtherTokenizers(keywords, openKeyword, inserted);
      _namedGroupIndice = BuildNamedGroupIndice(keywords);
      //_triggerKeyword = trigger;
    }

    public IEnumerable<Token> GetTokens(string source)
    {
      return GetTokens(source, 0, null).Where(o => o.Operation == TokenizeOperation.None);
    }
    
    #region Private Methods

    private IEnumerable<Token> GetTokens(string source, int startat, string? closingKey)
    {
      int i = startat;
      while (i < source.Length)
      {
        var match = Match(source, i);
        if (match == null)
          break;
        var token = InvokeFollowUps(source, match!, closingKey);
        if(token != null)
          yield return token;
        i = match.End;
      }
    }

    private Token? InvokeFollowUps(string source, PatternMatch match, string? closingKey)
    {
      var index = match.Index;
      foreach(var followUp in _followUps[match.KeywordIndex])
      {
        switch (followUp.Type)
        {
          case KeywordType.Intact: 
            return new Token(followUp.SyntaxCode, match.Argument, GetTextPosition(index), match.Length, null);
          case KeywordType.IntactAndMarkInside:
            
            break;
          case KeywordType.OpenLifo:
            var open = (MatchGateFollowUp)followUp;
            if (open.Tokenizer != null)
            {
              //need to make private Context
              //var children = GetTokens(source, match.End, followUp.ClosingKey);
            }
            else
            {
              var children = new List<Token>();
              Token? last = null;
              foreach (var e in GetTokens(source, match.End, open.ClosingKey))
              {
                if(e.Operation == TokenizeOperation.Fail || e.Operation == TokenizeOperation.Close)
                  break;
                children.Add(e);
                last = e;
              }
              if(last.Operation != TokenizeOperation.Close)
                return new Token(GetTextPosition(index), match.Length, TokenizeOperation.Fail);

              return new Token(followUp.SyntaxCode, match.Argument, GetTextPosition(index), last.Position.Index + last.Length - index, children);
            }
            
            break;
            //return  성공시 이걸 자식으로 가져야함.
          case KeywordType.OpenCloseFifo:

          
            break;
          case KeywordType.Close:
            var close = (MatchGateFollowUp)followUp;
            if (closingKey == close.ClosingKey)
              return new Token(GetTextPosition(index), match.Length, TokenizeOperation.Close);
            break;
          case KeywordType.Fail:
            //if 
            return new Token(GetTextPosition(index), match.Length, TokenizeOperation.Fail);
            //break;
        }
      }
      return null;
    }

    private TextPosition GetTextPosition(int index)
    {
      return new TextPosition(0, 0, index+1);//수정 필요
    }

    private PatternMatch? Match(string source, int startat)
    {
      var m = _regex.Match(source, startat);
      for (int i = 0; i < _patterns.Length; i++)
      {
        if (m.Groups[_namedGroupIndice[i]].Success)
        {
          var pattern = _patterns[i];
          var argument = pattern.CreateArgument(m.Groups.Cast<Group>().Skip(_namedGroupIndice[i]+1).Take(pattern.GroupNum-1).Select(o => o.Value).ToArray());

          return new PatternMatch(source, m.Index, m.Length, argument, i);
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

    private static List<MatchFollowUp>[] BuildFollowUpSet(Keyword[] keywords)
    {
      var ret = new List<MatchFollowUp>[keywords.Length];
      for (int i = 0; i < ret.Length; i++)
      {
        ret[i] = new List<MatchFollowUp>();
        ret[i].Add(keywords[i].FollowUp);
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

    /*private static List<Tokenizer> BuildOtherTokenizers(Keyword[] keywords, Keyword? openKeyword, Tokenizer? inserted)
    {
      var ret = new Tokenizer[keywords.Length];
      for (int i = keywords.Length - 1; i >= 0; i--)
      {
        if (keywords[i] == inserted._triggerKeyword)
          ret[i] = inserted;

        if (!keywords[i].HasPrivateContext || keywords[i] == openKeyword)
          continue;
        
        var subKeywords = new List<Keyword>();
        if (keywords[i].FollowUp.Markable)
        {
          subKeywords.Add();
          //heading fail 조건
        }
        subKeywords.Add(keywords[i]);
          
        var closer = keywords[i].FollowUp.ClosingKey;
          
        subKeywords.Add();
        //keywords[i].FollowUp.ClosingKey로 키워드 생성
          
        ret[i] = new Tokenizer(subKeywords.ToArray());
      }
    }*/

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