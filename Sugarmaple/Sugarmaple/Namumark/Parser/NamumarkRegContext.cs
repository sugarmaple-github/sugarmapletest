using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sugarmaple.Text;
using Sugarmaple.Namumark.Parser.Keywords;
using Sugarmaple.Namumark.Parser.Tokens;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamumarkRegContext
  {
    private readonly PatternInfo[] _patterns;
    private readonly Regex _regex;
    private readonly TokenCommand[][] _originCommands;
    private readonly List<TokenCommand>[] _overrideCommands;
    private readonly List<int> _namedGroupIndice;

    public NamumarkRegContext(params Keyword[] keywords)
    {
      _namedGroupIndice = BuildNamedGroupIndice(keywords);
      _patterns = keywords.Select(o => o.Pattern).ToArray();  
      _regex = BuildRegex(keywords);
      _originCommands = BuildOriginCommandSet(keywords);
      _overrideCommands = BuildOverrideCommandSet(keywords);
    }

    public PatternMatch? Match(string source, int startat, int length)
    {
      var m = _regex.Match(source, startat, length);
      for (int i = 0; i < _patterns.Length; i++)
      {
        for(int ii = 0; ii < 50; ii++)
          if(m.Groups[ii].Success)
            Console.WriteLine($"Match Success Group: {ii}");
        if (m.Groups[_namedGroupIndice[i]].Success)
        {
          var pattern = _patterns[i];
          var groupNum = 0;
          var argument = m.Groups.Cast<Group>()
            .Skip(_namedGroupIndice[i]+1).Take(pattern.GroupNum-1)
            .Select(o => new PatternGroup(source, o.Index, o.Length, GetOriginCommand(i, ++groupNum))).ToArray();
          return new PatternMatch(source, m.Index, m.Length, argument, GetOriginCommand(i, 0), _overrideCommands[i]);
        }
      }
      return null;
    }

    private TokenCommand GetOriginCommand(int keywordIndex, int groupIndex)
    {
      Console.WriteLine($"Get: {keywordIndex}, {groupIndex}");
      var ret = (keywordIndex < _originCommands.Length && groupIndex < _originCommands[keywordIndex].Length ? _originCommands[keywordIndex]?[groupIndex] : null) ?? TokenCommand.Empty;
      if(ret != TokenCommand.Empty)
        Console.WriteLine($"That's not Empty");
      return ret;
    }

    #region Constructor Helper Build Methods
    private static Regex BuildRegex(Keyword[] keywords)
    {
      var index = 0;
      var buffer = StringBuilderPool.Obtain()
        .Append('(').AppendJoin(")|(",
          Enumerable.Range(0, keywords.Length)
            .Select(i => keywords[i].Pattern.BuildRegex(_namedGroupIndice[i])))
        .Append(')');
      var regex = new Regex(buffer.ToString(), RegexOptions.Multiline | RegexOptions.Singleline);
      buffer.ToPool();
      return regex;
    }

    private static TokenCommand[][] BuildOriginCommandSet(Keyword[] keywords)
    {
      var ret = new TokenCommand[keywords.Length][];
      for (int i = 0; i < ret.Length; i++)
      {
        if(keywords[i].Command is ComplexCommand complex)
        {
          ret[i] = complex.Subcommands;
          for(int j = 0; j < ret[i].Length; j++)
          {
            Console.WriteLine($"Complex Detected {ret[i][j].SyntaxCode}, {ret[i][j].Type}");
            Console.WriteLine($"Set: {i}, {j}");
          }
        }
        else
          ret[i] = new TokenCommand[] { keywords[i].Command };
      }
      return ret;
    }
      
    private static List<TokenCommand>[] BuildOverrideCommandSet(Keyword[] keywords)
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
        var nextIndex = ret[i-1] + 1 + keywords[i-1].Pattern.GroupNum;
        ret.Add(nextIndex);
      }
      return ret;
    }
    #endregion
  }
}