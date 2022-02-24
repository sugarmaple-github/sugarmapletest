namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal enum TokenizeOperation: byte
  {
    None,
    Element,
    Open,
    Close,
    Fail,
  }
}