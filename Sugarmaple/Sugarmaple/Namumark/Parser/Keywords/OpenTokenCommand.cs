using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class OpenTokenCommand: TokenCommand
  {
    public delegate Context? ContextCallback(OpenTokenCommand self);

    public Context? Context { get; }
    private readonly object _closer;
    private readonly object[] _failKeys;

    public OpenTokenCommand(CommandType type, TokenCommand closer): this(type, SyntaxCode.None, closer) {}

    public OpenTokenCommand(CommandType type, SyntaxCode code, TokenCommand closer): this(type, code, closer, new List<TokenCommand>()) {}

    public OpenTokenCommand(CommandType type, SyntaxCode code, TokenCommand? closer, List<TokenCommand> fails, ContextCallback? callback = null): base(type, code)
    {
      _closer = closer ?? this;
      _failKeys = fails.ToArray();
      Context = callback?.Invoke(this);
    }

    public bool IsClosePartner(TokenCommand closer) => object.ReferenceEquals(_closer, closer);

    public bool IsFailPartner(TokenCommand fail) => _failKeys.Contains(fail);
  }
}