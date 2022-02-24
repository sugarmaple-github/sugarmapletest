using System.Collections.Generic;
using Sugarmaple.Dom;
using Sugarmaple.Namumark.Dom;
using Sugarmaple.Namumark.Parser.Tokens;
using static Sugarmaple.Namumark.Parser.SyntaxCode;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamuElementFactory
  {
    private static NamuElementFactory? _instance;
    public static NamuElementFactory Instance => _instance ??= new NamuElementFactory();

    private delegate NamuElement Creator(DomBasicArgument basic, PatternGroup[] sub);

    private readonly Dictionary<SyntaxCode, Creator> _creators = new ()
    {
      { Heading, (b, t) => new NamuHeadingElement(b, int.Parse(t[0].Raw), t[1] != null) },
    };

    public NamuElement Create(Wiki wiki, Document document, ElementToken token)
    {
      var basic = new DomBasicArgument(wiki, document, token.Index, token.Length, NodeType.Element);
      return _creators[token.SyntaxCode].Invoke(basic, token.Argument);
    }
  }
}