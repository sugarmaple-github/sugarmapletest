using System.Text;
using System.Collections.Generic;
using Sugarmaple.Text;

namespace Sugarmaple.Namumark.Parser
{
  internal sealed class Token
  {
    public SyntaxCode SyntaxCode { get; }
    public TokenArgument Argument { get; }
    public TextPosition Position { get; }
    public List<Token>? Children { get; }

    public Token(SyntaxCode code, TokenArgument argument, TextPosition position, List<Token>? children)
    {
      SyntaxCode = code;
      Argument = argument;
      Position = position;
      Children = children;
    }
  }
}