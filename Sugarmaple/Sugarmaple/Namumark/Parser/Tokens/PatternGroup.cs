using Sugarmaple.Namumark.Parser.Keywords;

namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class PatternGroup: FitCapture
  {
    public bool IsWikiBlock { get; }

    public PatternGroup(string text, int index, int length, bool isWikiBlock): base(text, index, length)
    {
      IsWikiBlock = isWikiBlock;
    }
  }
}