using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sugarmaple.Text
{
  public interface IRegexInfo
  {
    string Raw { get; }
    int GroupNum { get; }
  }

  public sealed class RegexBuilder: IRegexInfo, IDisposable
  {
    private readonly StringBuilder _buffer = StringBuilderPool.Obtain();
    string IRegexInfo.Raw => ToString();
    public int GroupNum { get; private set; }

    public void Const(char value, int repeatCount = 1) => AppendEscaping(value, repeatCount);
    public void Const(string value) => Append(Regex.Escape(value));
    public void LineStart() => Append('^');
    public void LineEnd() => Append("\\n");
    public void Any() => Append('.');

    public void ZeroOrOne() => Append('?');
    public void ZeroOrMore() => Append('*');
    public void FromMtoN(int m, int n) => Append('{').Append(m).Append(',').Append(n).Append('}');

    public void Class(params char[] chars) => Append('[').Append(chars).Append(']');
    public void Class(char m, char n) => Append('[').Append(m).Append('-').Append(n).Append(']');
    public void Group(string value) => StartGroup().Append(value).Append(')');
    public void Group(Action callback) { StartGroup(); callback(); Append(')'); }
    public void Group(IRegexInfo value)
    {
      StartGroup().Append(value.Raw).Append(')');
      GroupNum += value.GroupNum;
    }
    public void NonCaptureGroup(string value) => StartGroup().Append('?').Append(':').Append(value).Append(')');
    public void GroupAlternative(params string[] values) => GroupAlternative((IEnumerable<string>)values); //values don't have any regex groups.
    public void GroupAlternative(IEnumerable<string> values) => StartGroup().AppendJoin('|', values).Append(')');
    public void GroupAlternative(IEnumerable<IRegexInfo> builders)
    {
      StartGroup().AppendJoin('|', builders.Select(o => o.Raw)).Append(')');
      GroupNum += builders.Sum(o => o.GroupNum);
    }

    private StringBuilder StartGroup()
    {
      GroupNum++;
      return Append('(');
    }
    
    private void AppendEscaping(char value, int repeatCount = 1)
    {
      if (value.IsRegexMeta()) 
      {
        while(repeatCount > 0)
        {
          Append('\\');
          Append(value);
          repeatCount--;
        }
      } else _buffer.Append(value, repeatCount);
    }

    internal StringBuilder Append(char value) => _buffer.Append(value);
    private StringBuilder Append(string value) => _buffer.Append(value);
    private StringBuilder Append(char[] value) => _buffer.Append(value);

    public void Dispose()
    {
        _buffer.ToPool();
    }

    public override string ToString()
    {
      return _buffer.ToString();
    }
    //public static implicit operator string?(RegexBuilder? o) => o?.ToString();
  } 
}
