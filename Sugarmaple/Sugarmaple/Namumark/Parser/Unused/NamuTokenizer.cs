#if false
using System.Collections.Generic;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamuTokenizer
  {
    public NamuTokenizer(string source)
    {
      this.source = source;
      basicKeywords = Namumark.Config;
    }

    private readonly string source;
    private int pos = 0;

    private readonly IEnumerable<NamuKeyword> basicKeywords;
    private List<NamuKeyword> keywords;

    private readonly List<NamuToken> tokenBuffer = new List<NamuToken>();
    private bool IsMainTokenizing => tokenBuffer.Count == 0;
    private bool AllTokenComplete => beginNum == 0; 
    private int beginNum = 0;

    public NamuToken? Get()
    {
      return GetToken();
    }

    private void StartPhase(Regex regex, string source, int startIndex, bool isLevel0)
    {
      var m = regex.Match(source, startIndex);
      
      
    }

    private NamuToken GetToken() 
    {
      //이 위치에서의 구문의 성공, 실패 여부
      var token = CheckKeywordThisPos();
      if(beginNum == 0)
      {
        return token;
      }


    }


    private NamuToken? CheckKeywordThisPos()
    {
      foreach(var keyword in keywords)
      {
        if(!keyword.Regex.TryMatchHere(this, out var match))
          continue;

        if (keyword is NamuCreatorKeyword creator)
        {
          var token = creator.Create(match);
          if(creator.Type == KeywordType.Complete)
          {
            Progress(match.Length);
            return token;
          }

          BeginCSTokenize(creator);
        }

        
        //토큰이 생성될 수도 있음.
        //  (기본 파싱 중이라면 반환)
        //  (서브 파싱 중이라면 버퍼로)
        //현재 서브 파싱이 성공하여 반환할 수 있음.
        //현재 서브 파싱이 실패하여 위치를 '진행'시키고 다시 기본 파싱을 시작할 수 있음.
      }
    }

    private void BeginCSTokenize(NamuCreatorKeyword creator)
    {
      keywords = creator.Type == KeywordType.Begin ?
          creator.NewKeywords : defaultKeywords.Append(creator.NewKeywords);
    }

    private bool ConveyToken(NamuToken token, bool isComplete)
    {
      //creator.Create()
      //생성에 성공한다면

      //생성에 실패한다면
      return false;
    }

    //진행된 이후에는 플래그를 박아둔 것을 제외하고 되돌리지 않기를 원칙으로 함.
    private void Progress()
    {

    }
  }
}
#endif