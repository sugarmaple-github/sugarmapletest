using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Sugarmaple.Text
{
  internal static class StringExtension
  {
    static Stack<StringBuilder> pool = new();

    const string RegexMeta = @"\*+?|{[()^$.#";
    public static bool IsRegexMeta(this char c) => RegexMeta.Contains(c);
    public static char GetReverse(this char c)
    {
      return c switch {
        '(' => ')',
        '[' => ']',
        '{' => '}',
        _ => c,
      };
    }

    public static bool HasOnlyAscii(this string value)
    {
      foreach(var c in value)
        if(c > sbyte.MaxValue)
          return false;
      return true;
    }
  }
}