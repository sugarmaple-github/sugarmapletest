using System.Text.Json;

namespace Sugarmaple.Web
{
  public struct EditResponse: System.IDisposable
  {
    internal EditResponse(JsonDocument rootElement)
    {
      originData = rootElement;
    }

    private readonly JsonDocument originData;
    public string? Status => GetProperty("status").GetString();
    public int Rev
    {
      get
      {
        var output = GetProperty("rev").GetInt32();
        return output;
      }
    }

    private JsonElement GetProperty(string name) => originData.RootElement.GetProperty(name);

    public override string ToString() => originData.RootElement.ToString();

    public void Dispose()
    {
      originData.Dispose();
    }
  }
  
  public enum EditStatus
  {
    Success,
    ValidatorBadRequest, //권한이 없는 문서를 편집하려고 시도하면 발생 //"validator_bad_request"
    SameContent, //"문서 내용이 같습니다."
  }
}