using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Tokens;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamuToken: StringRange
  {
    public SyntaxCode SyntaxCode { get; }
    public PatternGroup[] Argument { get; }
    public IEnumerable<NamuToken> Children { get; internal set; }

    public NamuToken(SyntaxCode code, PatternGroup[] argument, int index, int length, IEnumerable<NamuToken>? children) : base(index, length)
    {
      SyntaxCode = code;
      Argument = argument;
      Children = children ?? new NamuToken[0];
    }
  }
}