namespace Sugarmaple.Dom
{
  public class Document: Node
  {
    internal Document(Wiki ownerWiki, string title): base(ownerWiki, NodeType.Document)
    {
      //Title = title;
    }

    public DocumentTitle Title { get; }
  }

  public struct DocumentTitle
  {
    Namespace Namespace;
    string Name { get; }
  }

  public class Namespace
  {

  }

  public class DocumentText
  {
    public DocumentText(string content)
    {
      
    }
  }
}