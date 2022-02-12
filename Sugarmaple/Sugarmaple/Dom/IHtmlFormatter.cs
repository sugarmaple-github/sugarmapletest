namespace Sugarmaple.Dom
{
  public interface IHtmlFormatter
  {
    string Comment();

    string LiteralText();

    string Text();

    string OpenTag(IElement element, bool selfClosing);

    string CloseTag(IElement element, bool selfClosing);
  }
}