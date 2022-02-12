using Sugarmaple.Dom;

namespace Sugarmaple.Namumark.Dom
{
  public class NamuLinkElement : NamuElement
  {
    internal NamuLinkElement(DomBasicArgument argument, string destination, string anchor): base(argument)
    {
        Destination = destination;
        Anchor = anchor;
    }

    public string Destination { get; }
    public string Anchor { get; }
  }
}