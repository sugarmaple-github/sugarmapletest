using System.Text;

namespace Sugarmaple.Namumark.Parser
{
  internal sealed class NamuToken
  {
    public SyntaxCode SyntaxCode { get; }
    public TokenType Type { get; }

    public NamuToken(SyntaxCode code, TokenType type)
    {
      SyntaxCode = code;
      Type = type;
    }
  }
}