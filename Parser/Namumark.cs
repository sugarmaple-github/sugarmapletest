#if false
using System.Collection.Generic;

namespace Sugarmaple.Namumark.Parser
{
  internal static class Namumark
  {
    static readonly NamuTokenCreator[] config = new[] {Macro};
    static IEnumerable<NamuTokenCreator> Config => config;

    static NamuTokenCreator Macro { get; } =
      new NamuTokenCreator(nameof(Macro), MacroRegex, true);

    static TokenRegex MacroRegex { get; } = 
     RegexBuilder.Create()
      .Const('[')
      .GroupBegin("Type").Options("age", "br", "clearfix", "date", "datetime", "dday", "footnote", "include", "ruby", "tableofcontents", "각주", "목차").GroupEnd()
      .GroupBegin(true).Const('(').GroupBegin("Argument").Const(')').GroupEnd()
      .Const(']')

    static TokenRegex MacroRegex { get; } = 
  }
}
#endif