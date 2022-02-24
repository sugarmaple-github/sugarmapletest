using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Keywords;

namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class PatternMatch: PatternGroup
  {
    public PatternGroup[] Groups { get; }
    public IEnumerable<TokenCommand> Commands { get; }

    public PatternMatch(string text, int index, int length, PatternGroup[] groups, IEnumerable<TokenCommand> commands): base(text, index, length, false)
    {
      Groups = groups;
      Commands = commands;
    }
  }
}