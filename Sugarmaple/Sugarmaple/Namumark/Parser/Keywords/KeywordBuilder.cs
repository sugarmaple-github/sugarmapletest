using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static Sugarmaple.Namumark.Parser.Keywords.SegmentOptions;
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
    private List<PatternGroup> groups = new List<PatternGroup>();


    public KeywordBuilder(SyntaxCode code)
    {
      _code = code;
      _headBuffer = StringBuilderPool.Obtain();
      _tailBuffer = new Stack<char>();
    }

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

    public KeywordBuilder BothEnd(char value, int minRepeat, int maxRepeat, SegmentOptions options)
    {
      string groupContent = (minRepeat == 0 && maxRepeat == 1) ? $"{value}?" : $"{value}{{{minRepeat},{maxRepeat}}}";
      var groupIndex = MakeGroup(groupContent, ToPatternGroup(options));
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

    public KeywordBuilder Options(params char[] chars)
    {
      Append('[');
      foreach(var o in chars)
        Append(o);
      Append(']');
      return this;
    }

    public KeywordBuilder Options(params KeywordBuilder[] children)
    {
      foreach(var o in children)
        _headBuffer.Append(o).Append('|');
      _headBuffer.Length--;
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

    public KeywordBuilder Group(SegmentOptions options, params string[] rawOptions)
    {
      MakeGroup(() => {
        foreach(var o in rawOptions)
          _headBuffer.Append(Regex.Escape(o)).Append('|');
        _headBuffer.Length--;
      }, ToPatternGroup(options));
      return this;
    }

    public KeywordBuilder Group(string content, SegmentOptions options)
    {
      MakeGroup(content, ToPatternGroup(options));
      return this;
    }

    public KeywordBuilder Group(params KeywordBuilder[] options) => Group(SegmentOptions.None, options.Select(o => o.ToString()).ToArray());

    public KeywordBuilder GroupBetween(char border, SegmentOptions options)
    {
      return GroupBetween(border, MatchedChar(border), options);
    }

    public KeywordBuilder GroupBetween(char border, int repeatCount, SegmentOptions options)
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

    public KeywordBuilder GroupBetween(char open, char close, SegmentOptions options)
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

    public KeywordBuilder GroupUntil(char until, SegmentOptions options)
    {
      MakeSlot(options);
      AppendEscaping(until);
      return this;
    }

    public KeywordBuilder GroupUntilLineEnd(SegmentOptions options)
    {
      options |= SegmentOptions.SingleLine;
      const string EndLine = @"\n?$";
      MakeSlot(options);
      Append(EndLine);
      return this;
    }

    #endregion

    //They make Keyword
    #region End Methods
    public (Keyword open, Keyword close) Lifo(SegmentOptions options = SegmentOptions.None) => Lifo(options, false, null);

    public (Keyword open, Keyword close) LifoPrivate(SegmentOptions options = SegmentOptions.None, params Keyword[] keywords) => Lifo(options, true, keywords);

    private (Keyword open, Keyword? close) Lifo(SegmentOptions options, bool isPrivate, Keyword[]? keywords)
    {
      var tailPattern = GetTailPattern();
      Keyword? close = null;
      string closingKey = null;
      if(tailPattern != null)
      {
        closingKey = tailPattern.Raw;
        var closeFollowUp = new MatchGateFollowUp(KeywordType.Close, SyntaxCode.None, null, closingKey);
        close = new Keyword(tailPattern, closeFollowUp);
      }
      else
      {
        foreach (var k in keywords)
          if(k.FollowUp.Type == KeywordType.Close)
          {
            close = k;
            closingKey = k.Pattern.Raw;
            break;
          }
      }
      
      Keyword? open;
      MatchGateFollowUp openFollowUp;

      if (isPrivate)
      {
        open = null;
        openFollowUp = new MatchGateFollowUp(KeywordType.OpenLifo, _code, Callback, closingKey);
        return (open!, close);
      }
      else
      {
        openFollowUp = new MatchGateFollowUp(KeywordType.OpenLifo, _code, closingKey);
        open = new Keyword(GetHeadPattern(), openFollowUp);
        return (open, close);
      }

      Tokenizer Callback(MatchGateFollowUp openFollowUp)
      {
        open = new Keyword(GetHeadPattern(), openFollowUp);
        return new Tokenizer(open, close);
      }
    }

    /*public Keyword LifoPrivate(SyntaxCode code, Keyword escape, PatternInfo failurePattern, SegmentOptions options)
    {
      var keywords = new List<Keyword>();
      var failure = new Keyword(failurePattern, KeywordType.Fail);
      var close = new Keyword(GetTailPattern(), KeywordType.Close);
      var open = new Keyword(GetHeadPattern(), KeywordType.OpenLifo, o => new KeywordGroup(o, escape, failure, close), code);
      return open;
    }*/

    public Keyword Fifo(SegmentOptions options)
    {
      return new Keyword(GetHeadPattern(), KeywordType.OpenCloseFifo, _code);
    }

    public Keyword Escape()
    {
      Append('.');
      return new Keyword(GetPattern());
    }

    public Keyword AccumulateAsList()
    {
      return new Keyword(GetPattern(), KeywordType.AccumulateAsList, _code);
    }

    public Keyword Intact()
    {
      return new Keyword(GetPattern(), KeywordType.Intact, _code);
    }

    public static KeywordBuilder Create(SyntaxCode code = SyntaxCode.None) => new KeywordBuilder(code);
    public static Keyword Failer(Keyword keyword) => new Keyword(keyword.Pattern, KeywordType.Fail);

    public override string ToString()
    {
      while (_tailBuffer.Count > 0)
        _headBuffer.Append(_tailBuffer.Pop());
      return _headBuffer.ToString();
    }
    #endregion
    private const string EscapeBackslash = @"(?<!\\)(\\\\)*";

    #region Private Functions
    private int MakeSlot(SegmentOptions options) => MakeGroup(options.HasFlag(SingleLine) ? @"[^\n]*?": @".*?", ToPatternGroup(options));

    private int MakeGroup(string content, PatternGroup ptGroup)
    {
      Append('(');
      Append(content);
      Append(')');
      groups.Add(ptGroup);
      return ++groupNum;
    }

    private int MakeGroup(Action callback, PatternGroup ptGroup)
    {
      Append('(');
      callback();
      Append(')');
      groups.Add(ptGroup);
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

    #region basic operator
    private void Append(char value) => _headBuffer.Append(value);
    private void Append(char value, int repeatCount) => _headBuffer.Append(value, repeatCount);
    private void Append(string value) => _headBuffer.Append(value);
    private void PrependTail(char value) => _tailBuffer.Push(value);
    #endregion
    #endregion

    private PatternGroup ToPatternGroup(SegmentOptions options)
    {
      if(options.HasFlag(SegmentOptions.Tag))
        return PatternGroup.Tag;
      if(options.HasFlag(SegmentOptions.Parameter))
        return PatternGroup.Parameter;
      if(options.HasFlag(SegmentOptions.Level))
        return PatternGroup.Level;
      return PatternGroup.None;
    }

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
      return new PatternInfo(raw, 1, isAccumulating, groups.ToArray());
    }

    private PatternInfo GetPattern()
    {
      var raw = ToString();
      return CreatePattern(raw);
    }

    private PatternInfo CreatePattern(string raw)
    {
      return new PatternInfo(raw, groupNum, isAccumulating, groups.ToArray());
    }
    #endregion
    #endregion
  }
}