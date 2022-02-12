using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class Keyword
  {
    public Keyword(PatternInfo pattern, MatchFollowUp followUp)
    {
      Pattern = pattern;
      FollowUp = followUp;
    }

    public Keyword(PatternInfo pattern, KeywordType type = KeywordType.None, SyntaxCode code = SyntaxCode.None): this(pattern, new MatchFollowUp(type, code)) {}

    public PatternInfo Pattern { get; }
    public MatchFollowUp FollowUp { get; }
  }
}