/*using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Keywords;
using static Sugarmaple.Namumark.Parser.Keywords.SlotOptions;
using static Sugarmaple.Namumark.Parser.Keywords.KeywordBuilder;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SugarmapleUnitTest")]
namespace Sugarmaple.Namumark.Parser
{
  internal static class Namumark
  {
    //private NamuTokenizer(): base(CreateKeywords()) {}
    
    private static Context? _instance;
    public static Context Instance => _instance ??= CreateContext();
    public static Tokenizer GetTokenizer(string source)
    {
      return new Tokenizer(Instance, source);
    }

    private static readonly string[] MacroNames = new[] {
      "age", "br", "clearfix", "date", "datetime", "dday", "footnote", "include", "kakaotv", "navertv", "nicovideo", "pagecount", "ruby", "tableofcontents", "youtube", "각주", "목차"
    };

    private static Context CreateContext()
    {
      Keyword Heading = Create(SyntaxCode.Heading).LineStart()
        .BothEndGroup(Create().Const('=').FromMtoN(1, 6))
        .BothEndGroup(Create().Const('=').ZeroOrOne())
        .GroupBetween(' ', Markable).Intact();
      (Keyword Open, Keyword Close) LiteralBrace = Create(SyntaxCode.LiteralBrace).BothEnd('{', 3).LifoPrivate();

      Keyword Escape = Create().Const('\\').Escape();

      (Keyword Open, Keyword Close) MarkupBrace = Create(SyntaxCode.MarkupBrace).Const('{', 3)
        .GroupAlt(
          Create().GroupAlt(Create().Class('+', '-').FromMtoN(1, 5)),
          Create().GroupAlt(@"#!wiki", @"#!folding").GroupUntilLineEnd())
        .LifoPrivate(Markable, Escape, Failer(Heading), LiteralBrace.Close);

      Keyword LinkOneLine = Create(SyntaxCode.Link).GroupBetween('[', 2, SingleLine).Intact();
      
      Keyword Macro = Create(SyntaxCode.Macro).BothEnd('[').GroupAlt(MacroNames).GroupBetween('(', Optional).Intact();

      (Keyword Open, Keyword Close) Link = Create(SyntaxCode.Link).BothEnd('[', 2).GroupUntil('#', SingleLine).GroupUntil('|', SingleLine).Lifo();

      (Keyword Open, Keyword Close) Footnote = Create(SyntaxCode.Footnote).BothEnd('[').GroupBetween('*', ' ', SingleLine).Lifo();

      

    /*static Keyword SyntaxBraceRegex { get; } = Create()
      .BorderRecursive('{', 3).Const("#!syntax ").Group("basic", "cpp", "csharp", "css", "erlang", "go", "java", "javascript", "json", "kotlin", "lisp", "lua", "markdown", "objectivec", "perl", "php", "powershell", "python", "ruby", "rust", "sh", "sql", "swift", "typescript", "xml").Const(' ').Group();*/

    //static Keyword Table { get; } = ;
    //^|()| || || ||
    // ||
    /*static Keyword NoMarkTripleBraceRegex { get; } = Create()
      .BorderRecursive{'{', 3}.Group(@"#!html", null).Group();*/

      /*Keyword Comment = Create(SyntaxCode.Comment).LineStart().Const('#', 2).GroupUntilLineEnd().Intact();
      Keyword List = Create().LineStart()
        .GroupAlt(
          Create(SyntaxCode.List).Const(' ').GroupAlt("*", Create().Class('1', 'A', 'I').Const('.')).Const(' ').ZeroOrOne().AccumulateAsList(),
          Create(SyntaxCode.List).Class(' ', '>').AccumulateAsLine()
        ).ZeroOrMore()
        .GroupAlt(
          Create(SyntaxCode.Horizon).Const('-').FromMtoN(4, 9).LineEnd().Intact(),
          Create(SyntaxCode.Table).Const('|').Fifo(SingleLine)
        ).Complex();
      //Keyword Indent = LineStart(true).Const(' ').GroupOptions()
    //구문이 언제 끝나는가?

      Keyword Bold = Create(SyntaxCode.Bold).BothEnd('\'', 3).Fifo(SingleLine);
      Keyword Italic = Create(SyntaxCode.Italic).BothEnd('\'', 2).Fifo(SingleLine);
      Keyword UnderLine = Create(SyntaxCode.UnderLine).BothEnd('_', 2).Fifo(SingleLine);
      Keyword StrikeThrough = Create(SyntaxCode.StrikeThrough).BothEnd('~', 2).Fifo(SingleLine);
      Keyword StrikeThrough2 = Create(SyntaxCode.StrikeThrough).BothEnd('-', 2).Fifo(SingleLine);
      Keyword Superscript = Create(SyntaxCode.Superscript).BothEnd('^', 2).Fifo(SingleLine);
      Keyword Subscript = Create(SyntaxCode.Subscript).BothEnd(',', 2).Fifo(SingleLine);
      Keyword NewLine = KeywordBuilder.NewLine;
      return new Context(Heading, Macro, MarkupBrace.Open, LiteralBrace.Open, Link.Open, LinkOneLine, Link.Close, Footnote.Open, Footnote.Close,
      Escape, Comment, List, Bold, Italic, UnderLine, StrikeThrough, StrikeThrough2, Superscript, Subscript, NewLine);
    }
    
    #region Simplify Function
    static KeywordBuilder Create(SyntaxCode code = SyntaxCode.None)
    {
      return new KeywordBuilder(code);
    }
    #endregion
  }
}*/