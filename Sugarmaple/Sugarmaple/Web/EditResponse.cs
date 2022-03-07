using System.Text.Json;

namespace Sugarmaple.Web
{
  public struct EditResponse
  {
    public string Status { get; }
    public int Rev { get; }
  }
  
  public enum EditStatus
  {
    Success,
    ValidatorBadRequest, //권한이 없는 문서를 편집하려고 시도하면 발생 //"validator_bad_request"
    SameContent, //"문서 내용이 같습니다."
  }
}