using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class ComplexCommand: TokenCommand
  {
    //Command By SubGroup
    public TokenCommand[] Subcommands { get; }

    public TokenCommand this[int index] => Subcommands[index];

    public ComplexCommand(params TokenCommand[] subcommands): base(CommandType.Complex, SyntaxCode.None)
    {
      Subcommands = subcommands;
    }
  }
}