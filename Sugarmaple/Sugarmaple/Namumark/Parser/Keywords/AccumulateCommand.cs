using System;
using System.Collections.Generic;
using System.Linq;
using Sugarmaple.Namumark.Parser;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class AccumulateCommand: TokenCommand
  {
    public delegate char SymbolFinder(PatternCapture capture);

    private readonly SymbolFinder _callback;
    private readonly TokenCommand? _lineAccumulator;
    
    public char GetSymbol(PatternCapture capture) => _callback(capture);

    public AccumulateCommand(CommandType type, SymbolFinder callback, TokenCommand? lineAccumulator): base(type, SyntaxCode.None)
    {
      _callback = callback;
      _lineAccumulator = lineAccumulator;
    }
  }
}