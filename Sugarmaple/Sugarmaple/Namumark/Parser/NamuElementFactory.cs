using System.Collections.Generic;
using Sugarmaple.Dom;
using Sugarmaple.Namumark.Dom;
using static Sugarmaple.Namumark.Parser.SyntaxCode;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamuElementFactory
  {
    private static NamuElementFactory? _instance;
    public static NamuElementFactory Instance => _instance ??= new NamuElementFactory();

    private delegate NamuElement Creator(DomBasicArgument argument, TokenArgument token);

    private readonly Dictionary<SyntaxCode, Creator> _creators = new ()
    {
      { Heading, (b, t) => new NamuHeadingElement(b, t.Level, t.Tag != null) },
    };

    public NamuElement Create(Wiki wiki, Document document, Token token)
    {
      var basic = new DomBasicArgument(wiki, document, token.Position, token.Length, NodeType.Element);
      return _creators[token.SyntaxCode].Invoke(basic, token.Argument);
    }
  }
}