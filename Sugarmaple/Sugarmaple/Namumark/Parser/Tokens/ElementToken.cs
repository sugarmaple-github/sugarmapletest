using System.Collections.Generic;

namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class ElementToken: Token
  {
    //public delegate IEnumerable<ElementToken> ChildrenFactory();
    
    public SyntaxCode SyntaxCode { get; }
    public PatternGroup[] Argument { get; }
    public IEnumerable<ElementToken> Children { get; }

    //private ChildrenFactory _factory;
    //private IEnumerable<ElementToken>? _children = null;
    //public IEnumerable<ElementToken> Children => _children ??= _factory.Invoke();

    /*public ElementToken(SyntaxCode code, string[] argument, int index, int length, List<ElementToken>? children = null) : this(code, argument, index, length, children, null) {}
    public ElementToken(SyntaxCode code, string[] argument, int index, int length, ChildrenFactory factory) : base(index, length, TokenizeOperation.None, ) {}

    private ElementToken(SyntaxCode code, string[] argument, int index, int length, List<ElementToken>? children, ChildrenFactory? factory) : base(index, length, TokenizeOperation.None)
    {
      SyntaxCode = code;
      Argument = argument;
      _children = children;
      Task.Run(_children = factory?.Invoke())
      //_factory = factory ?? delegate { yield break; };
    }*/

    public ElementToken(SyntaxCode code, PatternGroup[] argument, int index, int length, IEnumerable<ElementToken>? children) : base(index, length, TokenizeOperation.None)
    {
      SyntaxCode = code;
      Argument = argument;
      Children = children ?? new ElementToken[0];
      //Task.Run(_children = factory?.Invoke())
      //_factory = factory ?? delegate { yield break; };
    }
  }
}