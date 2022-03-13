using System;
using System.Collections.Generic;
using System.Linq;
using static Sugarmaple.Namumark.Parser.Keywords.SlotOptions;
using Sugarmaple.Text;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class KeywordBuilder: IDisposable
  {
    private readonly SyntaxCode _code;
    private readonly RegexBuilder _head = new RegexBuilder();
    private readonly Stack<char> _tail = new Stack<char>();
    //indice is reverse and +1
    private readonly List<int> _tailBackRefIndice = new List<int>();
    private readonly List<TokenCommand> _groupCommands = new();
    private bool isAccumulating = false;

    public KeywordBuilder(SyntaxCode code) => _code = code;

    public static Keyword NewLine = new Keyword(new PatternInfo(@"\n", 0, false), CommandType.Fail);

    #region Element Regex Methods
    public KeywordBuilder Const(char value, int repeatCount = 1)
    { _head.Const(value, repeatCount); return this; }
    public KeywordBuilder Const(string keyword)
    { _head.Const(keyword); return this; }
    public KeywordBuilder LineStart()
    { _head.LineStart(); return this; }
    public KeywordBuilder LineEnd()
    { _head.LineEnd(); return this; }
    public KeywordBuilder Class(params char[] chars)
    { _head.Class(chars); return this; }
    public KeywordBuilder Class(char m, char n)
    { _head.Class(m, n); return this; }
    public KeywordBuilder Any()
    { _head.Any(); return this; }
    public KeywordBuilder FromMtoN(int m, int n)
    { _head.FromMtoN(m, n); return this; }
    public KeywordBuilder ZeroOrOne()
    { _head.ZeroOrOne(); return this; }
    public KeywordBuilder ZeroOrMore()
    { _head.ZeroOrMore(); return this; }
    public KeywordBuilder Group(string content)
    { _head.Group(content); return this; }
    public KeywordBuilder GroupAlt(params string[] rawOptions)
    { _head.GroupAlternative(rawOptions); return this; }
    #endregion
    
    #region Advanced Methods
    public KeywordBuilder BothEnd(char value, int repeatCount = 1)
    {
      _head.Const(value, repeatCount);
      TailConst(value.GetReverse(), repeatCount);
      return this;
    }
    public KeywordBuilder BothEndGroup(KeywordBuilder builder)
    {
      TailPushGroup(_head.GroupNum);
      _head.Group(builder.ToRegexBuilder());
      return this;
    }

    public KeywordBuilder GroupAlt(params KeywordBuilder[] groups)
    {
      _head.GroupAlternative(groups.Select(o => o.ToRegexBuilder()));
      return this;
    }
    public KeywordBuilder GroupAlt(params Keyword[] groups)
    {
      _head.GroupAlternative(groups.Select(o => o.Pattern));
      _groupCommands.AddRange(groups.Select(o => o.Command));
      return this;
    }
    public KeywordBuilder GroupBetween(char border, SlotOptions options) => GroupBetween(border, 1, options);
    public KeywordBuilder GroupBetween(char border, int repeatCount, SlotOptions options = SlotOptions.None) => GroupBetween(border, border.GetReverse(), repeatCount, options);
    public KeywordBuilder GroupBetween(char open, char close, SlotOptions options = SlotOptions.None) => GroupBetween(open, close, 1, options);
    public KeywordBuilder GroupBetween(char open, char close, int repeatCount, SlotOptions options = SlotOptions.None)
    {
      if(options.HasFlag(Optional))
      {
        _head.Group(MakeInnerGroup);
        _head.ZeroOrOne();
      }
      else MakeInnerGroup();
      return this;

      void MakeInnerGroup()
      {
        _head.Const(open, repeatCount);
        GroupSlot(options);
        _head.Const(close, repeatCount);
      }
    }

    public KeywordBuilder GroupUntil(char until, SlotOptions options = SlotOptions.None)
    {
      GroupSlot(options);
      _head.Const(until);
      return this;
    }
    const string EndLine = @"\n?$";
    public KeywordBuilder GroupUntilLineEnd(SlotOptions options = SlotOptions.None)
    {
      options |= SlotOptions.SingleLine;
      GroupSlot(options);
      _head.LineEnd();
      return this;
    }

    #endregion

    //They make Keyword
    #region End Methods
    public (Keyword open, Keyword close) Lifo(SlotOptions options = SlotOptions.None) => Lifo(options, false, null);
    public (Keyword open, Keyword close) LifoPrivate(SlotOptions options = SlotOptions.None, params Keyword[] keywords) => Lifo(options, true, keywords);
    private (Keyword open, Keyword close) Lifo(SlotOptions options, bool isPrivate, Keyword[]? keywords)
    {
      var close = keywords?.Where(o => o.Command.Type == CommandType.Close).FirstOrDefault();
      var fails = new List<Keyword>();
      if (keywords != null)
        fails.AddRange(
          from k in keywords
          where k.Command.Type == CommandType.Fail
          select k
        );

      close ??= CreateKeywordTail(CreateCommand(CommandType.Close));
      var open = CreateOpenKeyword(close, fails, isPrivate);
      return (open, close);
    }

    public Keyword Fifo(SlotOptions options)
    {
      var command = options.HasFlag(SingleLine) ? CreateOpenCloseSelfCommand(CommandType.OpenCloseFifo, NewLine.Command) : CreateOpenCloseSelfCommand(CommandType.OpenCloseFifo);
      return CreateKeywordHead(command);
    }

    public Keyword Escape() => Any().CreateKeyword(CommandType.Intact);

    public Keyword AccumulateAsList() => CreateKeyword(CommandType.AccumulateAsList);
    public Keyword AccumulateAsLine() => CreateKeyword(CommandType.AccumulateAsLine);
    public Keyword Intact() => CreateKeyword(CommandType.Intact);
    public Keyword Complex() => CreateKeyword(CommandType.Complex);

    public static KeywordBuilder Create(SyntaxCode code = SyntaxCode.None) => new KeywordBuilder(code);
    public static Keyword Failer(Keyword keyword) => new Keyword(keyword.Pattern, CommandType.Fail);

    public RegexBuilder ToRegexBuilder()
    {
      foreach (var c in _tail)
        _head.Append(c);
      return _head;
    }

    public override string ToString()
    {
      var buffer = StringBuilderPool.Obtain();
      buffer.Append(_head);
      foreach (var c in _tail)
        buffer.Append(c);
      var output = buffer.ToString();
      buffer.ToPool();
      return output;
    }

    public void Dispose() => _head.Dispose();

    #endregion
    private const string EscapeBackslash = @"(?<!\\)(\\\\)*";

    #region Private Functions
    private int GroupSlot(SlotOptions options)
    {
      if (options.HasFlag(Markable))
      {
        AddGroupCommand(_head.GroupNum, CreateCommand(CommandType.Context));
        Console.WriteLine("Markable Slot Detected;");
      }
      _head.Group(options.HasFlag(SingleLine) ? @"[^\n]*?": @".*?");
      return _head.GroupNum;
    }
    
    #region Tail Push Functions
    private void TailPush(char value) => _tail.Push(value);
    private void TailPush(string value)
    {
      foreach(var c in value.Reverse())
        TailPush(c);
    }
    private void TailConst(char value, int repeatCount = 1)
    {
      while(repeatCount > 0)
      {
        TailPush(value);
        if(value.IsRegexMeta())
          TailPush('\\');
        --repeatCount;
      }
    }
    //vaild if 0 <= groupCode < 10
    private void TailPushGroup(int groupCode)
    {
      TailPush('>');
      TailPush('0' + groupCode);
      _tailBackRefIndice.Add(_tail.Count);  
      TailPush(@"\k<");
    }

    private void AddGroupCommand(int index, TokenCommand command)
    {
      while (_groupCommands.Count < index)
        _groupCommands.Add(TokenCommand.Empty);
      _groupCommands.Add(command);
    }
    #endregion
    #region Factory
    private Keyword CreateOpenKeyword(Keyword close, IEnumerable<Keyword> fails, bool isPrivate)
    {
      Keyword? open = null;
      new OpenTokenCommand(CommandType.OpenLifo, _code, close.Command, fails.Select(o => o.Command),
      (OpenTokenCommand o) => {
        open = new Keyword(GetHeadPattern(), o);
        return isPrivate ? new NamumarkRegContext(open!, close!) : null;
      });
      return open!;
    }

    private Keyword CreateKeyword(CommandType type)
    {
      var baseCommand = CreateCommand(type);
      if(_groupCommands.Count > 0)
      {
        _groupCommands[0] = baseCommand;
        return new Keyword(GetFullPattern(), new ComplexCommand(_groupCommands.ToArray()));
      }
      return new Keyword(GetFullPattern(), baseCommand);
    }
    private Keyword CreateKeywordHead(TokenCommand command) => new Keyword(GetHeadPattern(), command);
    private Keyword CreateKeywordTail(TokenCommand command) => new Keyword(GetTailPattern(), command);

    private PatternInfo GetHeadPattern() => CreatePattern(_head.ToString(), _head.GroupNum);
    private PatternInfo GetTailPattern() => CreatePattern(string.Concat(_tail), 0);
    private PatternInfo GetFullPattern() => CreatePattern(ToString(), _head.GroupNum);
    private PatternInfo CreatePattern(string raw, int groupNum) => new PatternInfo(raw, groupNum, _tailBackRefIndice.Reverse().Select(o => raw.Length - o), isAccumulating);

    private TokenCommand CreateCommand(CommandType type) => new TokenCommand(type, _code);
    private OpenTokenCommand CreateOpenCloseSelfCommand(CommandType type, TokenCommand? fail = null)
    {
      var fails = new List<TokenCommand>();
      if (fail != null) fails.Add(fail);
      return new OpenTokenCommand(type, _code, null, fails);
    }
    #endregion
    #endregion
  }
}