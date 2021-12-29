using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Keywords;
using static Sugarmaple.Namumark.Parser.Keywords.SegmentOptions;

namespace Sugarmaple.Namumark.Parser
{
  internal static class Namumark
  {
    static readonly Keyword[] config = new[] {
      Heading, Comment, //Macro, SizeBrace, MultiLineBrace, LiteralBrace,
      //Bold, Italic, UnderLine, StrikeThrough, StrikeThrough2, Superscript, Subscript
      };
    public static IEnumerable<Keyword> Config => config;

    static readonly string[] MacroNames = new[] {
      "age", "br", "clearfix", "date", "datetime", "dday", "footnote", "include", "kakaotv", "navertv", "nicovideo", "pagecount", "ruby", "tableofcontents", "youtube", "각주", "목차"
    };

    static Keyword Heading { get; } = Create(SyntaxCode.Heading).Border('=', 1, 6).Border('#', Optional).Group(' ', Markable);
    
    static Keyword Macro { get; } = Create(SyntaxCode.Macro).Border('[').Options(MacroNames).Group('(', Optional);

    //Enum.GetNames<KnownColor>().Select()
    static Keyword SizeBrace { get; } = Create(SyntaxCode.SizeBrace).Bracket('{', 3, Markable).Const(@"[\+\-][1-5] ");

    static Keyword MultiLineBrace { get; } = Create(SyntaxCode.MultiLineBrace).Bracket('{', 3, Markable).Options(@"#!wiki",  @"#!folding").Group(' ', '\n');

    static Keyword LiteralBrace { get; } = Create(SyntaxCode.LiteralBrace).BBracket('{', 3);
    /*static Keyword SyntaxBraceRegex { get; } = Create()
      .BorderRecursive('{', 3).Const("#!syntax ").Options("basic", "cpp", "csharp", "css", "erlang", "go", "java", "javascript", "json", "kotlin", "lisp", "lua", "markdown", "objectivec", "perl", "php", "powershell", "python", "ruby", "rust", "sh", "sql", "swift", "typescript", "xml").Const(' ').Group();*/
    
    //static Keyword Table { get; } = ;
    //^|()| || || ||
    // ||
    /*static Keyword NoMarkTripleBraceRegex { get; } = Create()
      .BorderRecursive{'{', 3}.Options(@"#!html", null).Group();*/

    static Keyword Comment { get; } = Line("##");
      //RegexBuilder.Create()
      //  .Level16
    static Keyword Bold           { get; } = BracketOneLevel('\'' , 3);
    static Keyword Italic         { get; } = BracketOneLevel('\'' , 2);
    static Keyword UnderLine      { get; } = BracketOneLevel('_'  , 2);
    static Keyword StrikeThrough  { get; } = BracketOneLevel('-'  , 2);
    static Keyword StrikeThrough2 { get; } = BracketOneLevel('~'  , 2);
    static Keyword Superscript    { get; } = BracketOneLevel('^'  , 2);
    static Keyword Subscript      { get; } = BracketOneLevel(','  , 2);
    
    #region Simplify Function
    static KeywordBuilder Border(char value) => Create().Border(value);
    static KeywordBuilder Border(char value, int from, int until) => Create().Border(value, from, until);
    static KeywordBuilder Bracket(char value, int repeatCount, SegmentOptions options) => Create().Bracket(value, repeatCount, options);
    //static KeywordBuilder BracketMarkable(char value, int repeatCount) => Create().BracketMarkable(value, repeatCount);
    //static KeywordBuilder BracketOneLevel(char value, int repeatCount) => Create().BracketOneLevel(value, repeatCount);

    static KeywordBuilder Line(string open) => Create().Line(open);

    static KeywordBuilder Create(SyntaxCode code) => new KeywordBuilder(code).Create();
    #endregion
  }
}

//(((?'Open'\{\{\{)\{{0,2}[^{]*)+((?'Close-Open'}}})[^<>]*)+)*(?(Open)(?!))$