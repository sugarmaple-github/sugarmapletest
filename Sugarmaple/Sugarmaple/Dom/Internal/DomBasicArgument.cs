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

    public DomBasicArgument(Wiki wiki, Document? document, TextPosition position, int length)
    {
      Wiki = wiki;
      Document = document;
      Position = position;
      Length = length;
    }
  }
}