using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Sugarmaple.Text
{
  internal static class StringBuilderPool
  {
    static Stack<StringBuilder> pool = new Stack<StringBuilder>();
    
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