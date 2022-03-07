using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class ComplexCommand: TokenCommand
  {
    //Command By SubGroup
    public TokenCommand[] _subcommands { get; }

    public TokenCommand this[int index] => _subcommands[index];

    public ComplexCommand(params TokenCommand[] subcommands): base(CommandType.Complex, SyntaxCode.None)
    {
      _subcommands = subcommands;
    }
  }
}