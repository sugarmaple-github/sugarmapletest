/*
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  //토큰의 판정대상과 토큰의 생성 메서드를 저장하는 클래스
  //미사용 예정
  internal class TokenKeyword: Keyword
  {
    public TokenKeyword(SyntaxCode code, string regexRaw, TokenType tokenType)
    :base(regexRaw, KeywordType.Token)
    {
      SyntaxCode = code;
      TokenType = tokenType;
    }

    public SyntaxCode SyntaxCode { get; }
    public TokenType TokenType { get; }

    public NamuToken Create(TokenMatch match) 
    {
      return new NamuToken(SyntaxCode, TokenType);
    }
  }
}*/