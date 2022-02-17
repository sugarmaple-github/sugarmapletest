namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class PatternMatch
  {
    public string Source;
    public int Index;
    public int Length { get; }
    public int End => Index + Length;
    public string Raw => Source[Index..End];

    public TokenArgument Argument { get; }
    public int KeywordIndex { get; }

    public PatternMatch(string source, int index, int length, TokenArgument argument, int keywordIndex)
    {
      Source = source;
      Index = index;
      Length = length;
      Argument = argument;
      KeywordIndex = keywordIndex;
    }
  }
}