namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class Token
  {
    public int Index { get; }
    public int Length { get; }
    public int End => Index + Length;
    public TokenizeOperation Operation { get; }

    public Token(int index, int length, TokenizeOperation operation)
    {
      Index = index;
      Length = length;
      Operation = operation;
    }
  }
}