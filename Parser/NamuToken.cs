using System.Text;

namespace Sugarmaple.Namumark.Parser
{
  internal sealed class NamuToken
  {
    public string Name { get; }
    public TokenType Type { get; }

    public NamuToken(string name, TokenType type)
    {
      Name = name;
      Type = type;
    }
  }
}