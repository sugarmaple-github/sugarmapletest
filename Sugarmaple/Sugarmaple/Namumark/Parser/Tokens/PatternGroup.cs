using Sugarmaple.Namumark.Parser.Keywords;

namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class PatternGroup
  {
    public string Text { get; }
    public int Index { get; }
    public int Length { get; }
    public bool IsWikiBlock { get; }

    public int End => Index + Length;
    public string Raw => Text[Index..End];

    public PatternGroup(string text, int index, int length, bool isWikiBlock)
    {
      Text = text;
      Index = index;
      Length = length;
      IsWikiBlock = isWikiBlock;
    }
  }
}