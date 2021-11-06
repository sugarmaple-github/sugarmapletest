#if false
namespace Sugarmaple.Namumark.Parser
{
  //토큰의 판정대상과 토큰의 생성 메서드를 저장하는 클래스
  internal class NamuCreatorKeyword
  {
    public NamuTokenCreator(string name, TokenRegex regex, TokenType tokenType, IList<NamuKeyword> newKeywords)
    :base(regex, KeywordType.Token)
    {
      TokenName = name;
      TokenType = tokenType;
      NewKeywords = newKeywords;
    }

    public string TokenName { get; }
    public TokenType TokenType { get; }
    public IList<NamuKeyword>? NewKeywords { get; }

    public NamuToken Create(TokenMatch match) 
    {
      var type = Convert(Type);
      return new NamuToken(TokenName, TokenType);
    }
  }
}
#endif