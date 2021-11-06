#if false
namespace Sugarmaple.Namumark.Parser
{
  internal class NamuKeyword: INamuKeyword
  {
    public NamuKeyword(TokenRegex regex, KeywordType type)
    {
      Regex = regex;
      Type = type;
    }

    public TokenRegex Regex { get; }
    public KeywordType Type { get; }
  }

  //미확정
  internal interface INamuKeyword
  {
    TokenRegex Regex { get; }
    KeywordType Type { get; }
  }
}
#endif