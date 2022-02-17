using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class MatchFollowUp
  {
    public KeywordType Type { get; }
    public SyntaxCode SyntaxCode { get; }

    public MatchFollowUp(KeywordType type = KeywordType.None, SyntaxCode code = SyntaxCode.None)
    {
      Type = type;
      SyntaxCode = code;
    }
  }
}