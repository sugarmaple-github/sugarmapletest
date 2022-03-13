namespace Sugarmaple.Namumark.Parser
{
  internal class StringRange
  {
    public int Index { get; }
    public int Length { get; }
    public int End => Index + Length;

    public StringRange(int index, int length)
    {
      Index = index;
      Length = length;
    }
  }
}