using System.Collections.Generic;
using Sugarmaple.Dom;
using Sugarmaple.Namumark.Dom;

namespace Sugarmaple.Namumark.Parser
{
  internal class NamuElementFactory
  {
    private delegate NamuElement Creator(DomBasicArgument argument, Token token);

    private readonly Dictionary<SyntaxCode, Creator> creators = new ()
    {
      { Heading, (b, t) => new NamuHeadingElement() },
    };
  }
}