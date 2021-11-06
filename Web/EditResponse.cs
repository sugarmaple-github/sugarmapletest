namespace Sugarmaple.Web
{
  public record EditResponse(string Status, int Rev);
  
  //문서 내용이 같습니다.
  //-제곧내
  //validator_bad_request
  //- 편집 권한이 없는 문서를 편집하려고 시도하면 발생
}