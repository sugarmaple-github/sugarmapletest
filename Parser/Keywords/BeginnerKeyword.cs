namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class BeginnerKeyword: TokenKeyword
  {
    public BeginnerKeyword(SyntaxCode code, string regexRaw, Keyword closer)
    :base(name, regexRaw, TokenType.Begin)
    {
      Closer = closer;
    }

    public Keyword Closer { get; }
  }
}