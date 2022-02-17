using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class MatchGateFollowUp: MatchFollowUp
  {
    public delegate Tokenizer TokenizerCallback(MatchGateFollowUp self);

    //only valid if KeywordType == Open
    public Tokenizer? Tokenizer { get; }
    public string ClosingKey { get; }

    public MatchGateFollowUp(KeywordType type, string closingKey): this(type, SyntaxCode.None, closingKey) {}

    public MatchGateFollowUp(KeywordType type, SyntaxCode code, string closingKey): this(type, code, null, closingKey) {}

    public MatchGateFollowUp(KeywordType type, SyntaxCode code, TokenizerCallback? callback, string closingKey): base(type, code)
    {
      Tokenizer = callback?.Invoke(this);
      ClosingKey = closingKey;
    }
  }
}