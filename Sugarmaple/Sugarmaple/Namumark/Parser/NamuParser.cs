using System.Collections.Generic;
using Sugarmaple.Dom;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamuParser
  {
    private NamuTokenizer _namumark = NamuTokenizer.Get();

    public IEnumerable<INode> GetNodes(string source)
    {
      foreach(var token in _namumark.GetTokens(source))
      {
        GetNode(token);
      }
    }

    private INode GetNode(Token token)
    {
      switch (token.SyntaxCode)
      {
        case SyntaxCode.Heading:
          break;
        case SyntaxCode.Macro:
          break;
        case SyntaxCode.SizeBrace:
          break;
        case SyntaxCode.MultiLineBrace:
          break;
        case SyntaxCode.LiteralBrace:
          break;
        case SyntaxCode.Bold:
          break;
        case SyntaxCode.Italic:
          break;
        case SyntaxCode.UnderLine:
          break;
        case SyntaxCode.StrikeThrough:
          break;
        case SyntaxCode.Superscript:
          break;
        case SyntaxCode.Subscript:
          break;
        case SyntaxCode.Comment:
          break;
        case SyntaxCode.Footnote:
          break;
        case SyntaxCode.Link:
          break;
      }
    }
    //확정된 토큰을 노드와 엘리먼트로 파싱
  }
}