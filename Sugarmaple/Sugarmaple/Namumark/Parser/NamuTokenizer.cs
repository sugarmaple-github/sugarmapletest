using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Keywords;
using static Sugarmaple.Namumark.Parser.Keywords.SegmentOptions;
using static Sugarmaple.Namumark.Parser.Keywords.KeywordBuilder;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SugarmapleUnitTest")]
namespace Sugarmaple.Namumark.Parser
{
  internal class NamuTokenizer: Tokenizer
  {
    private NamuTokenizer(): base(CreateKeywords())
    {
      
    }
    
    private static NamuTokenizer? _instance;
    public static NamuTokenizer Instance => _instance ??= new NamuTokenizer();

    private static readonly string[] MacroNames = new[] {
      "age", "br", "clearfix", "date", "datetime", "dday", "footnote", "include", "kakaotv", "navertv", "nicovideo", "pagecount", "ruby", "tableofcontents", "youtube", "각주", "목차"
    };

    private static Keyword[] CreateKeywords()
    {
      Keyword Heading = Create(SyntaxCode.Heading).LineStart().BothEnd('=', 1, 6, Level).BothEnd('#', 0, 1, Tag).GroupBetween(' ', Markable).Intact();
      (Keyword Open, Keyword Close) LiteralBrace = Create(SyntaxCode.LiteralBrace).BothEnd('{', 3).LifoPrivate();

      Keyword Escape = Create().Const('\\').Escape();

      (Keyword Open, Keyword Close) MarkupBrace = Create(SyntaxCode.MarkupBrace).Const('{', 3)
        .Options(
          Create().Group(Create().Options('+', '-').Range(1, 5)),
          Create().Group(Tag, @"#!wiki", @"#!folding").GroupUntilLineEnd(Parameter))
        .LifoPrivate(Markable, Escape, Failer(Heading), LiteralBrace.Close);

      Keyword LinkOneLine = Create(SyntaxCode.Link).GroupBetween('[', 2, Tag | SingleLine).Intact();
      
      Keyword Macro = Create(SyntaxCode.Macro).BothEnd('[').Group(Tag, MacroNames).GroupBetween('(', Parameter | Optional).Intact();

      (Keyword Open, Keyword Close) Link = Create(SyntaxCode.Link).BothEnd('[', 2).GroupUntil('#', SingleLine | Tag).GroupUntil('|', Parameter | SingleLine).Lifo();

      (Keyword Open, Keyword Close) Footnote = Create(SyntaxCode.Footnote).BothEnd('[').GroupBetween('*', ' ', SingleLine | Tag).Lifo();

      

    /*static Keyword SyntaxBraceRegex { get; } = Create()
      .BorderRecursive('{', 3).Const("#!syntax ").Group("basic", "cpp", "csharp", "css", "erlang", "go", "java", "javascript", "json", "kotlin", "lisp", "lua", "markdown", "objectivec", "perl", "php", "powershell", "python", "ruby", "rust", "sh", "sql", "swift", "typescript", "xml").Const(' ').Group();*/

    //static Keyword Table { get; } = ;
    //^|()| || || ||
    // ||
    /*static Keyword NoMarkTripleBraceRegex { get; } = Create()
      .BorderRecursive{'{', 3}.Group(@"#!html", null).Group();*/

      Keyword Comment = Create(SyntaxCode.Comment).LineStart().Const('#', 2).GroupUntilLineEnd(Parameter).Intact();
      Keyword List = Create(SyntaxCode.List).LineStart(true).Const(' ').Group(Tag, "*", "1.", "A.", "I.").ConstOption(' ').AccumulateAsList();
      //Keyword Indent = LineStart(true).Const(' ').GroupOptions()
    //구문이 언제 끝나는가?

      Keyword Bold = Create(SyntaxCode.Bold).BothEnd('\'', 3).Fifo(SingleLine);
      Keyword Italic = Create(SyntaxCode.Italic).BothEnd('\'', 2).Fifo(SingleLine);
      Keyword UnderLine = Create(SyntaxCode.UnderLine).BothEnd('_', 2).Fifo(SingleLine);
      Keyword StrikeThrough = Create(SyntaxCode.StrikeThrough).BothEnd('~', 2).Fifo(SingleLine);
      Keyword StrikeThrough2 = Create(SyntaxCode.StrikeThrough).BothEnd('-', 2).Fifo(SingleLine);
      Keyword Superscript = Create(SyntaxCode.Superscript).BothEnd('^', 2).Fifo(SingleLine);
      Keyword Subscript = Create(SyntaxCode.Subscript).BothEnd(',', 2).Fifo(SingleLine);
      return new Keyword[] {Heading, Macro, MarkupBrace.Open, LiteralBrace.Open, Link.Open, Link.Close, LinkOneLine, Footnote.Open, Footnote.Close,
      Escape, Comment, List, Bold, Italic, UnderLine, StrikeThrough, StrikeThrough2, Superscript, Subscript};
    }
    

    #region Simplify Function
    static KeywordBuilder Create(SyntaxCode code = SyntaxCode.None)
    {
      return new KeywordBuilder(code);
    }
    #endregion
  }
}