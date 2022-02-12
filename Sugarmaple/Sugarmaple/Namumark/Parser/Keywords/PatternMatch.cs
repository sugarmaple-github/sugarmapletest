namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class PatternMatch
  {
    public string Source;
    public int Index;
    public int Length { get; }
    public int End;
    public string Raw => Source[Index..End];

    public TokenArgument Argument { get; }
    public Keyword Keyword { get; }

    public PatternMatch(string source, int index, int length, TokenArgument argument, Keyword keyword)
    {
      Source = source;
      Index = index;
      Length = length;
      Argument = argument;
      Keyword = keyword;
    }
  }
}