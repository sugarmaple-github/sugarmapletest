using System;
using Sugarmaple;
using Sugarmaple.Attributes;

namespace Sugarmaple.Dom
{
  public interface INode
  {
    Wiki OwnerWiki { get; }

    Document? OwnerDocument { get; }

    bool HasChildNodes { get; }
    //[DomName("baseURI")]
    //string BaseUri { get; }

    //Url? BaseUrl { get; } //구현 예정

    //[DomName("nodeName")] //구현 예정
    //string NodeName { get; }

    [DomName("contains")]
    bool Contains(INode otherNode);

    [DomName("childNodes")]
    INodeList ChildNodes { get; }

    [DomName("firstChild")]
    INode? FirstChild { get; }

    [DomName("lastChild")]
    INode? LastChild { get; }

    [DomName("nodeType")]
    NodeType NodeType { get; }

    string TextContent { get; }

    [DomName("parentNode")]
    INode? Parent { get; }
  }
}