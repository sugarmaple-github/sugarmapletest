using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Sugarmaple.Namumark.Parser.Keywords.SegmentOptions;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class KeywordBuilder
  {
    private readonly SyntaxCode code;
    private readonly StringBuilder buffer;
    private readonly Stack<char> bookedChars;
    private TokenKeyword? closer = null;

    public KeywordBuilder(SyntaxCode code)
    {
      this.code = code;
      buffer = StringBuilderPool.Obtain();
      bookedChars = new Stack<char>();
    }

    #region Builder Functions
    public KeywordBuilder Const(char c)
    {
      buffer.Append(c);
      return this;
    }

    public KeywordBuilder Const(string keyword)
    {
      buffer.Append(keyword);
      return this;
    }

    public KeywordBuilder Border(char value, SegmentOptions options = None)
    {
      AppendBothSide(value);
      return this;
    }

    public KeywordBuilder Border(char value, int repeatMin, int repeatMax)
    {
      
    }

    public KeywordBuilder BorderOptional(char value)
    {
      buffer.Append('(').Append(value).Append(')');
      return this;
    }

    public KeywordBuilder Bracket(char value, int repeatCount, SegmentOptions options = None)
    {
      Append(value, repeatCount);
      closer = new TokenKeyword(code, MatchedChar(value) * repeatCount, TokenType.End);
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
      return new TokenKeyword(code, GetRegex(), TokenType.Complete);
    }

    public static implicit operator Keyword(KeywordBuilder b)
    {
      return b.ToKeyword();
    }
  }
}