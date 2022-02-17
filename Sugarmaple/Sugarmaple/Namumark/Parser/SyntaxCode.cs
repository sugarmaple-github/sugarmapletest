namespace Sugarmaple.Namumark.Parser
{
  internal enum SyntaxCode: byte
  {
    None,
    Heading,
    Macro,
    MarkupBrace,
    LiteralBrace,
    Bold,
    Italic,
    UnderLine,
    StrikeThrough,
    Superscript,
    Subscript,
    Comment,
    Footnote,
    Link,
    List,
  }
}