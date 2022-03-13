namespace Sugarmaple.Namumark.Parser
{
  internal class PatternCapture: StringRange
  {
    public string Text { get; }
    public string Raw => Text[Index..End];

    public char this[int index] => Text[Index + index];

    public PatternCapture(string text, int index, int length): base(index, length)
    {
      Text = text;
    }
  }
}