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

    public PatternMatch? Match(string source, int startat, int length)
    {
      var m = _regex.Match(source, startat, length);
      for (int i = 0; i < _patterns.Length; i++)
      {
        if (m.Groups[_namedGroupIndice[i]].Success)
        {
          var pattern = _patterns[i];
          var groupIndex = 1;
          var argument = m.Groups.Cast<Group>()
            .Skip(_namedGroupIndice[i]+1).Take(pattern.GroupNum-1)
            .Select(o => new PatternGroup(source, o.Index, o.Length, groupIndex++ == pattern.MarkableGroup)).ToArray();

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