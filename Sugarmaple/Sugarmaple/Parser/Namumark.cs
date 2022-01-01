using System.Collections.Generic;
using Sugarmaple.Namumark.Parser.Keywords;
using static Sugarmaple.Namumark.Parser.Keywords.SegmentOptions;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SugarmapleUnitTest")]
namespace Sugarmaple.Namumark.Parser
{
  internal static class Namumark
  {
    private static Keyword[]? config;
    private static int nextGroup = 1;

    public static IEnumerable<Keyword> Config => config ??= new Keyword[] {
      Heading!, Comment!, Macro!, SizeBrace!, MultiLineBrace!, LiteralBrace!,
      Bold!, Italic!, UnderLine!, StrikeThrough!, StrikeThrough2!, Superscript!, Subscript!
    };

    private static readonly string[] MacroNames = new[] {
      "age", "br", "clearfix", "date", "datetime", "dday", "footnote", "include", "kakaotv", "navertv", "nicovideo", "pagecount", "ruby", "tableofcontents", "youtube", "각주", "목차"
    };

    static Keyword Heading { get; } = Create(SyntaxCode.Heading).BorderRange('=', 1, 6).Border('#', Optional).Group(' ', SingleLine | Markable);

    static Keyword Macro { get; } = Create(SyntaxCode.Macro).Border('[').Options(MacroNames).Group('(', Optional);

    static Keyword SizeBrace { get; } = Create(SyntaxCode.SizeBrace).Bracket('{', 3, Markable).Const(@"[\+\-][1-5] ");

    static Keyword MultiLineBrace { get; } = Create(SyntaxCode.MultiLineBrace).Bracket('{', 3, Markable).Options(@"#!wiki", @"#!folding").Group(' ', '\n');

    static Keyword LiteralBrace { get; } = Create(SyntaxCode.LiteralBrace).Bracket('{', 3);
    /*static Keyword SyntaxBraceRegex { get; } = Create()
      .BorderRecursive('{', 3).Const("#!syntax ").Options("basic", "cpp", "csharp", "css", "erlang", "go", "java", "javascript", "json", "kotlin", "lisp", "lua", "markdown", "objectivec", "perl", "php", "powershell", "python", "ruby", "rust", "sh", "sql", "swift", "typescript", "xml").Const(' ').Group();*/

    //static Keyword Table { get; } = ;
    //^|()| || || ||
    // ||
    /*static Keyword NoMarkTripleBraceRegex { get; } = Create()
      .BorderRecursive{'{', 3}.Options(@"#!html", null).Group();*/

    static Keyword Comment { get; } = Create(SyntaxCode.Comment).Line("##");
    //RegexBuilder.Create()
    //  .Level16
    static Keyword Bold { get; } = Create(SyntaxCode.Bold).BracketOneLevel('\'', 3);
    static Keyword Italic { get; } = Create(SyntaxCode.Italic).BracketOneLevel('\'', 2);
    static Keyword UnderLine { get; } = Create(SyntaxCode.UnderLine).BracketOneLevel('_', 2);
    static Keyword StrikeThrough { get; } = Create(SyntaxCode.StrikeThrough).BracketOneLevel('-', 2);
    static Keyword StrikeThrough2 { get; } = Create(SyntaxCode.StrikeThrough).BracketOneLevel('~', 2);
    static Keyword Superscript { get; } = Create(SyntaxCode.Superscript).BracketOneLevel('^', 2);
    static Keyword Subscript { get; } = Create(SyntaxCode.Subscript).BracketOneLevel(',', 2);

    #region Simplify Function
    static KeywordBuilder Create(SyntaxCode code)
    {
      return new KeywordBuilder(code, nextGroup, o => nextGroup = o);
    }
    #endregion
  }
}