namespace Sugarmaple.Dom
{
  public class Document: Node
  {
    internal Document(SeedWiki ownerWiki, string title): base(ownerWiki, this)
    {
      OwnerWiki = ownerWiki;
      //Title = title;
    }

    public SeedWiki OwnerWiki { get; }  
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