using System.Text.Json;
using System.Linq;

namespace Sugarmaple.Web
{
  public struct BacklinkResponse
  {
    public (string Namespace, int Count)[] Namespaces { get; }
    public (string Document, string Flags)[] Backlinks { get; }
    public string From { get; }
    public string Until { get; }
  }
}