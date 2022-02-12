using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Keywords;
using static Sugarmaple.Namumark.Parser.Keywords.SegmentOptions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SugarmapleUnitTest")]
namespace Sugarmaple.Namumark.Parser
{
  internal class NamuTokenizer: Tokenizer
  {
    private NamuTokenizer(): base(CreateKeywords()) { }
    
    private static NamuTokenizer instance;
    public static NamuTokenizer Get()
    {
      return instance ??= new NamuTokenizer();
    }

    private static readonly string[] MacroNames = new[] {
      "age", "br", "clearfix", "date", "datetime", "dday", "footnote", "include", "kakaotv", "navertv", "nicovideo", "pagecount", "ruby", "tableofcontents", "youtube", "각주", "목차"
    };

    private static Keyword[] CreateKeywords()
    {
      Keyword Heading = LineStart().BothEnd('=', 1, 6, Level).BothEnd('#', 0, 1, Tag).GroupBetween(' ', Markable).Intact(SyntaxCode.Heading);
      Keyword Macro = BothEnd('[').GroupOptions(Tag, MacroNames).GroupBetween('(', Parameter | Optional).Intact(SyntaxCode.Macro);

      Keyword SizeBrace = BothEnd('{', 3).Group(@"[\+\-][1-5]", Tag).Const(' ').LifoPrivate(SyntaxCode.SizeBrace, Markable);//키워드 그룹으로 묶을 때 헤딩과 이스케이프 적용

      Keyword MultiLineBrace = BothEnd('{', 3).GroupOptions(Tag, @"#!wiki", @"#!folding").GroupUntilLineEnd(Parameter).LifoPrivate(SyntaxCode.MultiLineBrace, Markable);

      Keyword LiteralBrace = BothEnd('{', 3).LifoPrivate(SyntaxCode.LiteralBrace);

      Keyword LinkOneLine = GroupBetween('[', 2, Tag | SingleLine).Intact(SyntaxCode.Link);

      (Keyword Open, Keyword Close) Link = BothEnd('[', 2).GroupUntil('#', SingleLine | Tag).GroupUntil('|', Parameter | SingleLine).Lifo(SyntaxCode.Link);

      (Keyword Open, Keyword Close) Footnote = BothEnd('[').GroupBetween('*', ' ', SingleLine | Tag).Lifo(SyntaxCode.Footnote);

      Keyword Escape = Const('\\').Escape();

    /*static Keyword SyntaxBraceRegex { get; } = Create()
      .BorderRecursive('{', 3).Const("#!syntax ").Group("basic", "cpp", "csharp", "css", "erlang", "go", "java", "javascript", "json", "kotlin", "lisp", "lua", "markdown", "objectivec", "perl", "php", "powershell", "python", "ruby", "rust", "sh", "sql", "swift", "typescript", "xml").Const(' ').Group();*/

    //static Keyword Table { get; } = ;
    //^|()| || || ||
    // ||
    /*static Keyword NoMarkTripleBraceRegex { get; } = Create()
      .BorderRecursive{'{', 3}.Group(@"#!html", null).Group();*/

      Keyword Comment = LineStart().Const('#', 2).GroupUntilLineEnd(Parameter).Intact(SyntaxCode.Comment);
    
      //Keyword List = LineStart(true).Const(' ').Const('*').ConstOption(' ').Accumulate();
    //구문이 언제 끝나는가?

      Keyword Bold = BothEnd('\'', 3).Fifo(SyntaxCode.Bold, SingleLine);
      Keyword Italic = BothEnd('\'', 2).Fifo(SyntaxCode.Italic, SingleLine);
      Keyword UnderLine = BothEnd('_', 2).Fifo(SyntaxCode.UnderLine, SingleLine);
      Keyword StrikeThrough = BothEnd('~', 2).Fifo(SyntaxCode.StrikeThrough, SingleLine);
      Keyword StrikeThrough2 = BothEnd('-', 2).Fifo(SyntaxCode.StrikeThrough, SingleLine);
      Keyword Superscript = BothEnd('^', 2).Fifo(SyntaxCode.Superscript, SingleLine);
      Keyword Subscript = BothEnd(',', 2).Fifo(SyntaxCode.Subscript, SingleLine);
      return new Keyword[] {Heading, Macro, SizeBrace, MultiLineBrace, LiteralBrace,  Link.Open, Link.Close, LinkOneLine, Footnote.Open, Footnote.Close,
      Escape, Comment, Bold, Italic, UnderLine, StrikeThrough, StrikeThrough2, Superscript, Subscript};
    }
    

    #region Simplify Function
    static KeywordBuilder Create()
    {
      return new KeywordBuilder();
    }

    static KeywordBuilder LineStart(bool isAccumulating = false)
    {
      return Create().LineStart(isAccumulating);
    }

    static KeywordBuilder BothEnd(char value, int repeatCount = 1)
    {
      return Create().BothEnd(value, repeatCount);
    }

    static KeywordBuilder BothEnd(char value, int minRepeat, int maxRepeat, SegmentOptions options)
    {
      return Create().BothEnd(value, minRepeat, maxRepeat, options);
    }

    static KeywordBuilder GroupBetween(char border, int repeatCount, SegmentOptions options) => Create().GroupBetween(border, repeatCount, options);

    static KeywordBuilder Const(char c)
    {
      return Create().Const(c);
    }

    #endregion
  }
}