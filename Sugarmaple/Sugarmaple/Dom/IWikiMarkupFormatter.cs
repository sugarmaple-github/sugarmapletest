namespace Sugarmaple.Dom
{
  public interface IWikiMarkupFormatter
  {
    string Comment();

    string LiteralText();

    string Text();

    string OpenTag(IElement element, bool selfClosing);

    string CloseTag(IElement element, bool selfClosing);
  }
}