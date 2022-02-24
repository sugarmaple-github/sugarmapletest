//helped from https://github.com/AngleSharp/AngleSharp/blob/devel/src/AngleSharp/Dom/Internal/Node.cs
using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using Sugarmaple.Text;

namespace Sugarmaple.Dom
{
  public class Node: INode
  {
    private readonly Wiki? _wiki;
    private readonly NodeType _type;

    private Document? _document;
    private Node? _parent;
    private NodeList _children;
    private TextPosition _pos;
    private int _index;
    private int _len;

    //for Document
    internal Node(Wiki? ownerWiki, NodeType type)
    {
      _wiki = ownerWiki;
      _type = type;
      _children = new NodeList();
    }

    internal Node(DomBasicArgument argument): this(argument.Wiki, argument.Type)
    {
      _document = argument.Document;
      _index = argument.Index;
      _len = argument.Length;
    }

    #region Interface Implement

    public Wiki? OwnerWiki => _wiki;
    public Document? OwnerDocument => _document;
    public NodeType NodeType => _type;

    public string? TextContent => _type switch {
      NodeType.Document => TextContent,
      NodeType.Element => OwnerDocument!.TextContent[_pos.Position..(_pos.Position+ _len)],//수정필요
      _ => null
    };

    public TextPosition Position => _pos;
    public int Length => _len;

    INode? INode.Parent => _parent;
    public bool HasChildNodes => _children.Length > 0;
    public bool Contains(INode otherNode) => otherNode.IsDescendantOf(this);

    INodeList INode.ChildNodes => _children;
    INode? INode.FirstChild => FirstChild;
    INode? INode.LastChild => LastChild;

    internal Node? FirstChild => _children.Length > 0 ? _children[0] : null;

    internal Node? LastChild => _children.Length > 0 ? _children[^1] : null;

    #endregion

    public void ToWikiMarkup(TextWriter writer, IWikiMarkupFormatter formatter)
    {
      //구현 예정
      //writer.Write();
    }

    public void ToHtml(TextWriter writer, IHtmlFormatter formatter)
    {
      //구현 예정
      /*foreach (var node in GetDescendantsAndSelf())
      {
        if(node is IElement element)
        {
          writer.Write(formatter.OpenTag(element));
        }
      }*/
    }

    #region Internal Properties

    internal Node? Parent { get => _parent; set => _parent = value; }

    #endregion
  }
}