/*using Sugarmaple.Attributes;

namespace Sugarmaple.Namumark.Dom
{
  [DomName("Node")]
  public interface IClauseNode : INode
  {
    [DomName("cloneNode")]
    INode Clone(Boolean deep = true);

    [DomName("isEqualNode")]
    bool Equals(INode otherNode);

    [DomName("compareDocumentPosition")]
    DocumentPositions CompareDocumentPosition(INode otherNode);

    [DomName("normalize")]
    void Normalize();

    [DomName("parentElement")]
    IElement? ParentElement { get; }

    [DomName("nextSibling")]
    INode? NextSibling { get; }

    [DomName("previousSibling")]
    INode? PreviousSibling { get; }

    [DomName("hasChildNodes")]
    [MemberNotNullWhen(true, nameof(ChildNodes), nameof(FirstChild), nameof(LastChild))]
    Boolean HasChildNodes { get; }

    [DomName("ownerDocument")]
    IDocument? OwnerDocument { get; }
  }
}*/