#if false
//This is copy from https://github.com/AngleSharp/AngleSharp/blob/devel/src/AngleSharp/Text/TextSource.cs
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sugarmaple.Text
{
  public sealed class TextSource: IDisposable
  {
    private readonly Stream baseStream;
    private readonly MemoryStream raw;
    private readonly byte[] buffer;
    private readonly char[] chars;

    private StringBuilder content;
    private bool finished;
    private int index;

    public char this[int index] => content[index]

    public char ReadChar()
    {
      return content[index];
    }
  } 
}
#endif