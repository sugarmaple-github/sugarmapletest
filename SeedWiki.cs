using System;

namespace Sugarmaple
{
  public class SeedWiki
  {
    public static SeedWiki TheSeedWiki { get; } = new SeedWiki("더시드위키", "https://theseed.io");

    public string Name { get; }
    public Uri Uri { get; }
    public string BaseAddress => Uri.OriginalString;
    //public SeedClient BaseClient { get; set; }

    public SeedWiki(string name, string baseAddress): this(name, new Uri(baseAddress)) {}

    public SeedWiki(string name, Uri uri)
    {
      Name = name;
      Uri = uri;
    }
  }
}