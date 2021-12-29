using System.Text.Json;
using System.Linq;

namespace Sugarmaple.Web
{
  public struct BacklinkResponse: System.IDisposable
  {
    public BacklinkResponse(JsonDocument data)
    {
      originData = data;
    }

    public (string Namespace, int Count)[] Namespaces => GetProperty("namespaces").EnumerateArray().Select(o => (o.GetProperty("namespace").GetString()!, o.GetProperty("count").GetInt32())).ToArray();
    public (string Document, string Flags)[] Backlinks => GetProperty("backlinks").EnumerateArray().Select(o => (o.GetProperty("document").GetString()!, o.GetProperty("flags").GetString()!)).ToArray();
    public string From => GetProperty("from").GetString()!;
    public string Until => GetProperty("until").GetString()!;

    private JsonElement GetProperty(string name) => originData.RootElement.GetProperty(name);

    private readonly JsonDocument originData;

    public override string ToString() => originData.RootElement.ToString();

    public void Dispose()
    {
      originData.Dispose();
    }
  }
}