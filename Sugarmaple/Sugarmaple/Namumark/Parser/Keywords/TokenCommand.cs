using System;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class TokenCommand
  {
    public CommandType Type { get; }
    public SyntaxCode SyntaxCode { get; }
    //아직은 개별 하위 구문이 하나인 경우만 산정 + 0이라면 없는 것

    public TokenCommand(CommandType type = CommandType.None, SyntaxCode code = SyntaxCode.None)
    {
      Type = type;
      SyntaxCode = code;
    }
  }
}