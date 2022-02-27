using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Sugarmaple.Text
{
  internal static class StringExtension
  {
    static Stack<StringBuilder> pool = new();
    
    public static StringBuilder Obtain()
    {
        if (pool.Count == 0)
        {
          return new StringBuilder(1024);
        }
      return pool.Pop().Clear();
    }

    public static void ToPool(this StringBuilder element)
    {
      pool.Push(element);
    }
  }
}