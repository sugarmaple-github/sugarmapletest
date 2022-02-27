using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Sugarmaple.Namumark.Parser.Keywords.SlotOptions;
using Sugarmaple.Text;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class KeywordBuilder
  {
    private readonly SyntaxCode _code;
    private readonly StringBuilder _headBuffer;
    private readonly Stack<char> _tailBuffer;
    private bool isAccumulating = false;
    private int groupNum = 1;
    private int markableGroup = 0;

    public KeywordBuilder(SyntaxCode code)
    {
      _code = code;
      _headBuffer = StringBuilderPool.Obtain();
      _tailBuffer = new Stack<char>();
    }

    public static Keyword NewLine = new Keyword(new PatternInfo(@"\n", 1, false, 0), CommandType.Fail);

    #region Build Methods
    //온전함
    //선입선출 = 디자인
    //후입선출 (개행 시 실패)
    //후입선출 + 다른 문맥 = 내부 열기/닫기 + 역슬래시 이스케이프 + 문단 포착시 실패
    //후입선출 + 다른 문맥 = 내부 열기/닫기
    //누적 개행 구문
    //말단 누적 개행 구문

    public KeywordBuilder Const(char c)
    {
      AppendEscaping(c);
      return this;
    }

    public KeywordBuilder Const(char c, int repeatCount)
    {
      AppendEscaping(c, repeatCount);
      return this;
    }

    public KeywordBuilder ConstOption(char c)
    {
      AppendEscaping(c);
      Append('?');
      return this;
    }

    public KeywordBuilder Const(string keyword)
    {
      Append(keyword);
      return this;
    }

    public KeywordBuilder LineEnd()
    {
      Append('\\');
      Append('n');
      return this;
    }

    public KeywordBuilder BothEnd(char value)
    {
      AppendBothSide(value);
      return this;
    }

    public KeywordBuilder BothEnd(char value, int repeatCount)
    {
      AppendBothSide(value, repeatCount);
      return this;
    }

    public KeywordBuilder BothEnd(char value, int minRepeat, int maxRepeat, SlotOptions options = SlotOptions.None)
    {
      string groupContent = (minRepeat == 0 && maxRepeat == 1) ? $"{value}?" : $"{value}{{{minRepeat},{maxRepeat}}}";
      var groupIndex = MakeGroup(groupContent);
      PrependTailGroup(groupIndex);
      return this;
    }

    public KeywordBuilder LineStart(bool cumulative = false)
    {
      if (cumulative)
        isAccumulating = true;
      else
        Append('^');
      return this;
    }

    public KeywordBuilder Class(params char[] chars)
    {
      Append('[');
      foreach(var o in chars)
        Append(o);
      Append(']');
      return this;
    }

    public KeywordBuilder Quantity(int from, int to)
    {
      Append('[');
      Append(from);
      Append(',');
      Append(to);
      Append(']');
      return this;
    }

    public KeywordBuilder Options(params KeywordBuilder[] children)
    {
      foreach(var o in children)
        _headBuffer.Append(o).Append('|');
      _headBuffer.Length--;
      groupNum += children.Select(o => o.groupNum - 1).Sum();
      return this;
    }

    public KeywordBuilder Range(byte from, byte to)
    {
      Append('[');
      _headBuffer.Append(from);
      Append('-');
      _headBuffer.Append(to);
      Append(']');
      return this;
    }

    public KeywordBuilder Group(params string[] rawOptions) => Group(SlotOptions.None, rawOptions);

    public KeywordBuilder Group(SlotOptions options, params string[] rawOptions)
    {
      MakeGroup(() => AppendEscaping(rawOptions, '|'));
      return this;
    }

    public KeywordBuilder Group(string content, SlotOptions options = SlotOptions.None)
    {
      MakeGroup(content);
      return this;
    }

    public KeywordBuilder Group(params KeywordBuilder[] options)
    {
      Group(SlotOptions.None, options.Select(o => o.ToString()).ToArray());
      groupNum += options.Select(o => o.groupNum - 1).Sum();
      return this;
    }

    public KeywordBuilder Group(params Keyword[] options)
    {
      var patterns = options.Select(o => o.Pattern);
      MakeGroup(() => Append(patterns.Select(o => o.Raw), '|'));
      Group(SlotOptions.None, options.Select(o => o.ToString()).ToArray());
      groupNum += options.Select(o => o.groupNum - 1).Sum();
      return this;
    }

    public KeywordBuilder GroupBetween(char border, SlotOptions options)
    {
      return GroupBetween(border, MatchedChar(border), options);
    }

    public KeywordBuilder GroupBetween(char border, int repeatCount, SlotOptions options = SlotOptions.None)
    {
      if(options.HasFlag(Optional))
      {
        MakeNonCapturingGroup(MakeInnerGroup);
        Append('?');
      }
      else MakeInnerGroup();
      return this;

      void MakeInnerGroup()
      {
        AppendEscaping(border, repeatCount);
        MakeSlot(options);
        AppendEscaping(MatchedChar(border), repeatCount);
      }
    }

    public KeywordBuilder GroupBetween(char open, char close, SlotOptions options = SlotOptions.None)
    {
      if(options.HasFlag(Optional))
      {
        MakeNonCapturingGroup(MakeInnerGroup);
        Append('?');
      }
      else MakeInnerGroup();
      return this;

      void MakeInnerGroup()
      {
        AppendEscaping(open);
        MakeSlot(options);
        AppendEscaping(close);
      }
    }

    public KeywordBuilder GroupUntil(char until, SlotOptions options = SlotOptions.None)
    {
      MakeSlot(options);
      AppendEscaping(until);
      return this;
    }

    const string EndLine = @"\n?$";
    public KeywordBuilder GroupUntilLineEnd(SlotOptions options = SlotOptions.None)
    {
      options |= SlotOptions.SingleLine;
      MakeSlot(options);
      Append("\n");
      return this;
    }

    #endregion

    //They make Keyword
    #region End Methods
    public (Keyword open, Keyword close) Lifo(SlotOptions options = SlotOptions.None) => Lifo(options, false, null);

    public (Keyword open, Keyword close) LifoPrivate(SlotOptions options = SlotOptions.None, params Keyword[] keywords) => Lifo(options, true, keywords);

    private (Keyword open, Keyword close) Lifo(SlotOptions options, bool isPrivate, Keyword[]? keywords)
    {
      var tailPattern = GetTailPattern();
      Keyword? close = null;
      var fails = new List<TokenCommand>();
      if(tailPattern != null)
      {
        var closeCommand = new TokenCommand(CommandType.Close);
        close = new Keyword(tailPattern, closeCommand);
      }
      else
      {
        foreach (var k in keywords!)
        {
          if (k.Command.Type == CommandType.Close)
          {
            close = k;
            break;
          }
          if (k.Command.Type == CommandType.Fail)
            fails.Add(k.Command);
        }
      }
      
      Keyword? open = null;
      var openCommand = new OpenTokenCommand(CommandType.OpenLifo, _code, close!.Command, fails,
      (OpenTokenCommand o) => {
        open = new Keyword(GetHeadPattern(), o);
        return isPrivate ? new Context(open!, close!) : null;
      });
      return (open!, close);
    }

    public Keyword Fifo(SlotOptions options)
    {
      OpenTokenCommand command;
      if (options.HasFlag(SingleLine))
        command = CreateOpenCloseSelf(CommandType.OpenCloseFifo, _code, NewLine.Command);
      else
        command = CreateOpenCloseSelf(CommandType.OpenCloseFifo, _code);
      return new Keyword(GetHeadPattern(), command);
    }

    public Keyword Escape()
    {
      Append('.');
      return new Keyword(GetPattern());
    }

    public Keyword AccumulateAsList()
    {
      return new Keyword(GetPattern(), CommandType.AccumulateAsList, _code);
    }

    public Keyword AccumulateAsLine()
    {
      return new Keyword(GetPattern(), CommandType.AccumulateAsLine, _code);
    }
    //(^( (\*|1\.|A\.|I\.) ?| |>)*(-{4,9}$|\|))
    //1. 하위 그룹별 Command가 별도로 필요. -> ComplexCommand 안에서 해결해보기
    //2. match 하나당 여러 번의 처리가 필요할 수도 있음.
      //그에 따라 별도 버퍼 필요성.
    
    public Keyword Intact()
    {
      return new Keyword(GetPattern(), CommandType.Intact, _code);
    }

    public static KeywordBuilder Create(SyntaxCode code = SyntaxCode.None) => new KeywordBuilder(code);
    public static Keyword Failer(Keyword keyword) => new Keyword(keyword.Pattern, CommandType.Fail);

    public override string ToString()
    {
      while (_tailBuffer.Count > 0)
        _headBuffer.Append(_tailBuffer.Pop());
      return _headBuffer.ToString();
    }
    #endregion
    private const string EscapeBackslash = @"(?<!\\)(\\\\)*";

    #region Private Functions
    private int MakeSlot(SlotOptions options)
    {
      var result = MakeGroup(options.HasFlag(SingleLine) ? @"[^\n]*?": @".*?");
      if(options.HasFlag(Markable))
        markableGroup = result - 1;
      return result;
    }

    private int MakeGroup(string content)
    {
      Append('(');
      Append(content);
      Append(')');
      return ++groupNum;
    }

    private int MakeGroup(Action callback)
    {
      Append('(');
      callback();
      Append(')');
      return ++groupNum;
    }

    private void MakeNonCapturingGroup(Action callback)
    {
      Append("(?:");
      callback();
      Append(')');
    }
    
    #region Append Functions
    private void AppendEscaping(char value)
    {
      if(IsMetachar(value)) Append('\\');
      Append(value);
    }

    private void AppendEscaping(char value, int repeatCount)
    {
      if(IsMetachar(value))
        for(int i = 0; i < repeatCount; i++)
          AppendEscaping(value);
      else
        Append(value, repeatCount);
    }

    private void Append(string[] items, char border)
    {
      foreach(var o in items)
        _headBuffer.Append(o).Append(border);
      _headBuffer.Length--;
    }

    private void AppendEscaping(string[] items, char border)
    {
      foreach(var o in items)
        _headBuffer.Append(Regex.Escape(o)).Append(border);
      _headBuffer.Length--;
    }

    private void PrependTail(string value)
    {
      foreach(var c in value.Reverse())
        PrependTail(c);
    }

    private void PrependTailEscaping(char value)
    {
      PrependTail(value);
      if(IsMetachar(value))
        PrependTail('\\');
    }

    private void PrependTailGroup(int groupCode) => PrependTail($@"\k<{groupCode}>");
    //(^( (\*|[1AI]\.) ?|[ >])*(-{4,9}$|\|))
    private void AppendBothSide(char value)
    {
      PrependTailEscaping(MatchedChar(value));
      AppendEscaping(value);
    }

    private void AppendBothSide(char value, int repeatCount)
    {
      AppendEscaping(value, repeatCount);
      for (var i = 0; i < repeatCount; i++)
        PrependTailEscaping(MatchedChar(value));
    }

    public static implicit operator string(KeywordBuilder o) => o.ToString();
    
    #region basic operator
    private void Append(char value) => _headBuffer.Append(value);
    private void Append(char value, int repeatCount) => _headBuffer.Append(value, repeatCount);
    private void Append(int value) => _headBuffer.Append(value);
    private void Append(string value) => _headBuffer.Append(value);
    private void PrependTail(char value) => _tailBuffer.Push(value);
    #endregion
    #endregion

    const string MetaChars = @"\*+?|{[()^$.#";
    private static bool IsMetachar(char c) => MetaChars.Contains(c);
    private static char MatchedChar(char c)
    {
      return c switch {
        '(' => ')',
        '[' => ']',
        '{' => '}',
        _ => c,
      };
    }
    
    #region Pattern Factory
    private PatternInfo GetHeadPattern()
    {
      var raw = _headBuffer.ToString();
      return CreatePattern(raw);
    }

    private PatternInfo GetTailPattern()
    {
      var _headBuffer = StringBuilderPool.Obtain();
      while (_tailBuffer.Count > 0)
        _headBuffer.Append(_tailBuffer.Pop());
      var raw = _headBuffer.ToString();
      _headBuffer.ToPool();
      return new PatternInfo(raw, 1, isAccumulating, markableGroup);
    }

    private PatternInfo GetPattern()
    {
      var raw = ToString();
      return CreatePattern(raw);
    }

    private PatternInfo CreatePattern(string raw)
    {
      return new PatternInfo(raw, groupNum, isAccumulating, markableGroup);
    }

    private static OpenTokenCommand CreateOpenCloseSelf(CommandType type, SyntaxCode code, TokenCommand? fail = null)
    {
      var fails = new List<TokenCommand>();
      if(fail != null)
        fails.Add(fail);
      return new OpenTokenCommand(type, code, null, fails);
    }
    #endregion
    #endregion
  }
}