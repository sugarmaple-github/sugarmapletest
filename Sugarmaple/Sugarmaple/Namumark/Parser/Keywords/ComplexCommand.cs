using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class ComplexCommand: TokenCommand
  {
    public TokenCommand[] Subcommands { get; }

    public ComplexCommand(params TokenCommand[] subcommands): base(CommandType.Complex, SyntaxCode.None)
    {
      Subcommands = subcommands;
    }
  }
}