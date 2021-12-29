using System.Text.RegularExpressions;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class Keyword
  {
    public Keyword(string regexRaw, KeywordType type)
    {
      RegexRaw = regexRaw;
      Type = type;
    }

    public string RegexRaw { get; }
    public KeywordType Type { get; }
  }
}