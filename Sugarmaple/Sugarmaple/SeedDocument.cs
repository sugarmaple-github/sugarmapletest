namespace Sugarmaple
{
  public class SeedDocument
  {
    internal SeedDocument(SeedWiki ownerWiki, string title)
    {
      OwnerWiki = ownerWiki;
      //Title = title;
    }

    public SeedWiki OwnerWiki { get; }  
    public SeedDocumentTitle Title { get; }
  }

  public struct SeedDocumentTitle
  {
    SeedNamespace Namespace;
    string Name;
  }

  public class SeedNamespace
  {

  }

  public class SeedDocumentText
  {
    public SeedDocumentText(string content)
    {
      
    }
  }
}