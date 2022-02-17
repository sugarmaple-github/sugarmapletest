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
    public int Length { get; }
    public List<Token>? Children { get; }
    public TokenizeOperation Operation { get; }

    public Token(SyntaxCode code, TokenArgument argument, TextPosition position, int length, List<Token>? children)
    {
      SyntaxCode = code;
      Argument = argument;
      Position = position;
      Length = length;
      Children = children;
      Operation = TokenizeOperation.None;
    }

    public Token(TextPosition position, int length, TokenizeOperation operation): this(SyntaxCode.None, default(TokenArgument), position, length, null)
    {
      Operation = operation;
    }
  }

  internal enum TokenizeOperation: byte
  {
    None,
    Success,
    Close,
    Fail,
  }
}