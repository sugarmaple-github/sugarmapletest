using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Keywords;

namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class PatternMatch: PatternGroup
  {
    public PatternGroup[] Groups { get; }
    public IEnumerable<TokenCommand> OverrideCommands { get; }

    public PatternMatch(string text, int index, int length, PatternGroup[] groups, TokenCommand command, IEnumerable<TokenCommand> overrideCommands): base(text, index, length, command)
    {
      Groups = groups;
      OverrideCommands = overrideCommands;
    }
  }
}