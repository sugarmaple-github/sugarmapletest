using Sugarmaple;
using Sugarmaple.Text;

namespace Sugarmaple.Dom
{
  internal readonly struct DomBasicArgument
  {
    public readonly Wiki Wiki;
    public readonly Document? Document;
    public readonly TextPosition Position;
    public readonly int Length;
    public readonly NodeType Type;

    public DomBasicArgument(Wiki wiki, Document? document, TextPosition position, int length, NodeType type)
    {
      Wiki = wiki;
      Document = document;
      Position = position;
      Length = length;
      Type = type;
    }
  }
}