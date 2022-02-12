using System;

namespace Sugarmaple
{
  public class Wiki
  {
    public static Wiki TheSeedWiki { get; } = new Wiki("더시드위키", new Uri("https://theseed.io"));

    public string Name { get; }
    public Uri Uri { get; }
    public string BaseAddress => Uri.OriginalString;
    //public SeedClient BaseClient { get; set; }

    public Wiki(string name, Uri uri)
    {
      Name = name;
      Uri = uri;
    }
  }
}