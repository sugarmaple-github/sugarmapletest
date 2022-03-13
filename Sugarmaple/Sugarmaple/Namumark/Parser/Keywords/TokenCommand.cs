using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class TokenCommand
  {
    public static TokenCommand Empty { get; } = new TokenCommand();
    public CommandType Type { get; }
    public SyntaxCode SyntaxCode { get; }
    
    public TokenCommand(CommandType type = CommandType.None, SyntaxCode code = SyntaxCode.None)
    {
      Type = type;
      SyntaxCode = code;
    }
  }
}