using Sugarmaple;
using Sugarmaple.Text;

namespace Sugarmaple.Dom
{
  internal readonly struct DomBasicArgument
  {
    public readonly Wiki Wiki;
    public readonly Document? Document;
    public readonly int Index;
    public readonly int Length;
    public readonly NodeType Type;

    public DomBasicArgument(Wiki wiki, Document? document, int index, int length, NodeType type)
    {
      Wiki = wiki;
      Document = document;
      Index = index;
      Length = length;
      Type = type;
    }
  }
}