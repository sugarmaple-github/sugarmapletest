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
    private readonly Keyword[] keywords;
    private readonly Regex regex;
    private readonly List<int> namedGroupIndice = new List<int>();

    public Tokenizer(params Keyword[] keywords)
    {
      this.keywords = keywords;
      regex = CreateRegexFromKeywords(keywords);
      var num = 1;
      namedGroupIndice.Add(1);
      for (var i = 1; i < this.keywords.Length; i++)
      {
        var nextIndex = namedGroupIndice[i-1] + this.keywords[i-1].Pattern.GroupNum;
        namedGroupIndice.Add(nextIndex);
      }
    }

    public IEnumerable<Token> GetTokens(string source)
    {
      return GetTokens(source, 0);
    }
        
    private IEnumerable<Token> GetTokens(string source, int startat)
    {
      int i = startat;
      while (i < source.Length)
      {
        var match = Match(source, startat);
        if (match == null)
          break;
        
        var keyword = match!.Keyword;
        var followUp = keyword.FollowUp;
        switch (followUp.Type)
        {
          case KeywordType.Intact: 
            yield return new Token(followUp.SyntaxCode, match.Argument, new TextPosition(0, 0, i+1));
            break;
          case KeywordType.OpenLifo:
            
            break;
          case KeywordType.Close:
            break;
        }
        i = match.Index + match.Length;
      }
    }

    private PatternMatch? Match(string source, int startat)
    {
      var m = regex.Match(source, startat);
      for (int i = 0; i < keywords.Length; i++)
      {
        if (m.Groups[namedGroupIndice[i]].Success)
        {
          var pattern = keywords[i].Pattern;
          var argument = pattern.CreateArgument(m.Groups.Cast<Group>().Skip(namedGroupIndice[i]+1).Take(pattern.GroupNum-1).Select(o => o.Value).ToArray());

          return new PatternMatch(source, m.Index, m.Length, argument, keywords[i]);
        }
      }
      return null;
    }

    private static Regex CreateRegexFromKeywords(Keyword[] keywords)
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
  }
}