namespace Sugarmaple.Web
{
  public class RedirectResponse
  {
    public (string Namespace, int Count)[] Namespaces { get; }
    public (string Document, string Flags)[] Backlinks { get; }
    public string From { get; }
    public string Until { get; }
  }
}