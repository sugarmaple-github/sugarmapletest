using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class MatchFollowUp
  {
    public KeywordType Type { get; }
    public SyntaxCode SyntaxCode { get; }
    public bool HasPrivateContext { get; }
    //only valid if KeywordType == Open
    public string? ClosingKey { get; }

    public MatchFollowUp(KeywordType type = KeywordType.None, SyntaxCode code = SyntaxCode.None): this(type, false, code) {}

    public MatchFollowUp(KeywordType type, bool hasPrivateContext, SyntaxCode code, string? closingKey = null)
    {
      Type = type;
      HasPrivateContext = hasPrivateContext;
      SyntaxCode = code;
      ClosingKey = closingKey;
    }
  }
}