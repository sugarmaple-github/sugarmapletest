using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class OpenTokenCommand: TokenCommand
  {
    public delegate NamumarkRegContext? ContextCallback(OpenTokenCommand self);

    public NamumarkRegContext? Context { get; }
    private readonly object _closer;
    private readonly object[] _failKeys;

    public OpenTokenCommand(CommandType type, TokenCommand closer): this(type, SyntaxCode.None, closer) {}

    public OpenTokenCommand(CommandType type, SyntaxCode code, TokenCommand closer): this(type, code, closer, new TokenCommand[0]) {}

    public OpenTokenCommand(CommandType type, SyntaxCode code, TokenCommand? closer, IEnumerable<TokenCommand> fails, ContextCallback? callback = null): base(type, code)
    {
      _closer = closer ?? this;
      _failKeys = fails.ToArray();
      Context = callback?.Invoke(this);
    }

    public bool IsClosePartner(TokenCommand closer) => object.ReferenceEquals(_closer, closer);

    public bool IsFailPartner(TokenCommand fail) => _failKeys.Contains(fail);
  }
}