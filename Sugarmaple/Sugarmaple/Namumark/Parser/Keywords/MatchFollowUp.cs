using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class MatchFollowUp
  {
    public KeywordType Type { get; }
    public SyntaxCode SyntaxCode { get; }
    //only true possible when KeywordType == Open
    //public bool HasPrivateContext { get; }//obsolete

    //only valid if KeywordType == Open,
    //public bool Markable { get; }//obsolete

    

    public MatchFollowUp(KeywordType type = KeywordType.None, SyntaxCode code = SyntaxCode.None)//: this(type, false, code) {}
    {
      Type = type;
      //HasPrivateContext = hasPrivateContext;
      SyntaxCode = code;
      //ClosingKey = closingKey;
    }

    /*public MatchFollowUp(KeywordType type, bool hasPrivateContext, SyntaxCode code, string? closingKey = null)
    {
      Type = type;
      HasPrivateContext = hasPrivateContext;
      SyntaxCode = code;
      ClosingKey = closingKey;
    }*/
  }
}