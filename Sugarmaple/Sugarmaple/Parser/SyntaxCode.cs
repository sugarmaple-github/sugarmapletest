namespace Sugarmaple.Namumark.Parser
{
  internal enum SyntaxCode: byte
  {
    Heading,
    Macro,
    SizeBrace,
    MultiLineBrace,
    LiteralBrace,
    Bold,
    Italic,
    UnderLine,
    StrikeThrough,
    Superscript,
    Subscript,
    Comment,
  }
}