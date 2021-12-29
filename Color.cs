#if false
namespace Sugarmaple.Namumark
{
  public struct DarkableColor
  {
    public string Light { get; }
    public string Dark { get; }

    public DarkableColor(string light, string dark)
    {
      Light = light;
      Dark = dark;
    }

    public static DarkableColor Parse(string value)
    {
      var m = Regex.Match(value, @"^(.*?),(.*?)$");
      var light = ColorTranslator.FromHtml(m.Groups[0].Value);
      var dark = ColorTranslator.FromHtml(m.Groups[1].Value);
      return new DarkableColor(light, dark);
    }

    public static string[] KnownColorNames = new[] {
      "aliceblue",
      
    };
  }
}
#endif