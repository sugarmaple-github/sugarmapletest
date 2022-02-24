using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class OpenTokenCommand: TokenCommand
  {
    public delegate Tokenizer? TokenizerCallback(OpenTokenCommand self);

    public Tokenizer? Tokenizer { get; }
    public object _closer;
    public object[] _failKeys;

    public OpenTokenCommand(CommandType type, TokenCommand closer): this(type, SyntaxCode.None, closer) {}

    public OpenTokenCommand(CommandType type, SyntaxCode code, TokenCommand closer): this(type, code, closer, new List<TokenCommand>()) {}

    public OpenTokenCommand(CommandType type, SyntaxCode code, TokenCommand closer, List<TokenCommand> fails, TokenizerCallback? callback = null): base(type, code)
    {
      _closer = closer;
      _failKeys = fails.ToArray();
      Tokenizer = callback?.Invoke(this);
    }

    public bool IsClosePartner(TokenCommand closer) => object.ReferenceEquals(_closer, closer);

    public bool IsFailPartner(TokenCommand fail) => _failKeys.Contains(fail);
  }
}