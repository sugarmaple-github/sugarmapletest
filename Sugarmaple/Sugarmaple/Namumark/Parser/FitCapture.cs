namespace Sugarmaple.Namumark.Parser
{
  internal class FitCapture
  {
    public string Text { get; }
    public int Index { get; }
    public int Length { get; }

    public int End => Index + Length;
    public string Raw => Text[Index..End];

    public char this[int index] => Text[Index + index];

    public FitCapture(string text, int index, int length)
    {
      Text = text;
      Index = index;
      Length = length;
    }
  }
}