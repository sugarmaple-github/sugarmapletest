using System;
using System.Text.Json;

namespace Sugarmaple.Web
{
  internal struct ViewResponse: IDisposable
  {
    public ViewResponse(JsonDocument data)
    {
      originData = data;
    }
    
    public string Text => GetProperty("text").GetString() ?? throw new ArgumentNullException();
    public bool Exists => GetProperty("exists").GetBoolean();
    public string Token => GetProperty("token").GetString() ?? throw new ArgumentNullException();
    private JsonElement GetProperty(string name) => originData.RootElement.GetProperty(name);

    private readonly JsonDocument originData;

    public override string ToString() => originData.RootElement.ToString();

    public void Dispose()
    {
      originData.Dispose();
    }
  }
}