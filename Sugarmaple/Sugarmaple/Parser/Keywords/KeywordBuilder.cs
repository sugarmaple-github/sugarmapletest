using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Sugarmaple.Namumark.Parser.Keywords.SegmentOptions;
using Sugarmaple.Text;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class KeywordBuilder
  {
    private readonly SyntaxCode code;
    private readonly StringBuilder buffer;
    private readonly Stack<char> bookedChars;
    private readonly int startGroup;
    private readonly Action<int> OnEnd;

    private TokenKeyword? closer = null;
    
    private int currentGroup;

    public KeywordBuilder(SyntaxCode code, int startGroup, Action<int> onEnd)
    {
      this.code = code;
      buffer = StringBuilderPool.Obtain();
      bookedChars = new Stack<char>();
      this.startGroup = startGroup;
      OnEnd = onEnd;
    }

    #region Builder Functions
    public KeywordBuilder Const(char c)
    {
      buffer.Append(c);
      return this;
    }

    public KeywordBuilder Const(string keyword)
    {
      Append(keyword);
      return this;
    }

    public KeywordBuilder Border(char value, SegmentOptions options = None)
    {
      if (options.HasFlag(Optional))
      {
        Append($"({value}?)");
      }
      else
        AppendBothSide(value);
      return this;
    }

    public KeywordBuilder BorderRange(char value, int repeatMin, int repeatMax)
    {
      Append($"({value}{{{repeatMin},{repeatMax}}})");
      return this;
    }

    public KeywordBuilder Bracket(char value, int repeatCount, SegmentOptions options = None)
    {
      Append(value, repeatCount);
      closer = new TokenKeyword(code, new string(MatchedChar(value), repeatCount), TokenType.End);
      //토큰 타입: begin, 새로운 토큰 추가:
      //throw new NotImplementedException()
      return this;
    }

    public KeywordBuilder BracketOneLevel(char value, int repeatCount)
    {
      Append(value, repeatCount);
      AppendBothSide(value, repeatCount);
      return this;
    }

    #region Group Series
    public KeywordBuilder Group(char border, SegmentOptions options = None)
    {
      AppendBothSide(border);
      Append('(');
      if(options.HasFlag(SingleLine))
        Append(@"[^\n]*");
      else
        Append(@".*");
      Append(')');
      if(options.HasFlag(Optional))
        Append('?');
      //SomeThing About Sub Mark Parsing
      return this;
    }

    public KeywordBuilder Group(char open, char close)
    {
      Append(@$"{open}([^\n]*){close}");
      return this;
    }

    private const string EndLine = @"\n?$";
    public KeywordBuilder Line(char value)
    {
      Append('^');
      Append(value);
      PrependTail(EndLine);
      return this;
    }

    public KeywordBuilder Line(string value)
    {
      Append('^');
      AppendBothSide(value, EndLine);
      return this;
    }
    #endregion

    //type 이름의 그룹으로 씌워진 노드를 붙입니다.
    public KeywordBuilder Options(params string[] options)
    {
      AppendBothSide('(');
      foreach(var o in options)
      {
        buffer.Append(o).Append('|');
      }
      buffer.Length--;
      return this;
    }
    #endregion

    #region Private Functions
    private void Append(char value) => buffer.Append(value);
    private void Append(char value, int repeatCount) => buffer.Append(value, repeatCount);
    private void Append(string value) => buffer.Append(value);

    private void PrependTail(string value)
    {
      foreach(var c in value.Reverse())
        bookedChars.Push(c);
    }

    private void PrependTailGroup(int groupCode) => PrependTail($@"\k<{groupCode}>");

    private void AppendBothSide(char value)
    {
      buffer.Append(value);
      bookedChars.Push(MatchedChar(value));
    }

    private void AppendBothSide(char value, int repeatCount)
    {
      buffer.Append(value);
      for (var i = 0; i < repeatCount; i++)
      {
        bookedChars.Push(MatchedChar(value));
      }
    }

    private void AppendBothSide(string open, string close)
    {
      buffer.Append(open);
      foreach(var c in close)
        bookedChars.Push(c);
    }

    private static char MatchedChar(char c)
    {
      return c switch {
        '(' => ')',
        '[' => ']',
        '{' => '}',
        _ => c,
      };
    }
    #endregion
    
    private string GetRegex()
    {
      while (bookedChars.Count > 0)
      {
        buffer.Append(bookedChars.Pop());
      }
      return buffer.ToString();
    }

    private Keyword ToKeyword()
    {
      OnEnd(currentGroup);
      return new TokenKeyword(code, GetRegex(), TokenType.Complete);
    }

    public static implicit operator Keyword(KeywordBuilder b)
    {
      return b.ToKeyword();
    }
  }
}