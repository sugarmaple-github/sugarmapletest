//This is copy from https://github.com/AngleSharp/AngleSharp/blob/devel/src/AngleSharp/Text/TextPosition.cs
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Sugarmaple.Text
{
  public readonly struct TextPosition
  {
    //base 1
    public int Line { get; }
    //base 1
    public int Column { get; }
    //base 1
    public int Position { get; }

    public int Index => Position - 1;


    public TextPosition(int line, int column, int position)
    {
      Line = line;
      Column = column;
      Position = position;
    }
  }
}