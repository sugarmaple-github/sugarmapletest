using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sugarmaple.Text
{
  public sealed class RegexBuilder
  {
    private StringBuilder _buffer = new StringBuilder();

    public void Const(char c) => AppendEscaping(c);
    public void LineStart() => Append('^');
    public void LineEnd() => Append("\\n");

    public void ZeroOrMore() => Append('*');
    public void ZeroOrOne() => Append('?');
    public void FromMtoN(int m, int n) => Append('{').Append(m).Append('.').Append(n).Append('}');

    public void Class(params char[] chars) => Append('[').Append(chars).Append(']'); 
    public void Group(string value) => Append('(').Append(value).Append(')');

    private void AppendEscaping(char value)
    {
      if(value.IsRegexMeta()) Append('\\');
      Append(value);
    }

    private StringBuilder Append(char value) => _buffer.Append(value);
    private StringBuilder Append(string value) => _buffer.Append(value);
    private StringBuilder Append(char[] value) => _buffer.Append(value);
  } 
}
