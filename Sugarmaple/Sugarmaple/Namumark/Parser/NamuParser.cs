using System.Collections.Generic;
using Sugarmaple.Dom;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamuParser
  {
    private readonly NamuTokenizer _tokenizer = NamuTokenizer.Instance;
    private readonly NamuElementFactory _elementFactory = NamuElementFactory.Instance;

    public IEnumerable<INode> GetNodes(Wiki wiki, Document document, string source)
    {
      foreach(var token in _tokenizer.GetTokens(source))
      {
        yield return _elementFactory.Create(wiki, document, token);
      }
    }
    //확정된 토큰을 노드와 엘리먼트로 파싱
  }
}