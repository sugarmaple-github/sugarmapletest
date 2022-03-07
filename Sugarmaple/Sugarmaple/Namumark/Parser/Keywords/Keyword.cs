using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class Keyword
  {
    public Keyword(PatternInfo pattern, TokenCommand command)
    {
      Pattern = pattern;
      Command = command;
    }

    public Keyword(PatternInfo pattern,
                   CommandType type = CommandType.None,
                   SyntaxCode code = SyntaxCode.None): this(pattern, new TokenCommand(type, code)) {}

    public PatternInfo Pattern { get; }
    public TokenCommand Command { get; }
  }
}