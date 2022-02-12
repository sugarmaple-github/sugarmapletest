using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SugarmapleUnitTest
{
  public static class TestStarter
  {
    const string AssemblyName = "SugarmapleUnitTest";

    public static void Start()
    {
      var assem = typeof(TestStarter).Assembly;
      var types = GetTypesWithAttribute<TestClassAttribute>(assem);
      foreach(var t in types)
      {
        var inst = Activator.CreateInstance(t);
        var methods = t.GetMethods().Where(m => m.GetCustomAttributes(typeof(TestMethodAttribute), false).Length > 0);
        foreach(var m in methods)
        {
          var attribute = (m.GetCustomAttributes(typeof(TestMethodAttribute)).First()) as TestMethodAttribute;
          if (attribute == null) continue;

          WriteLine($"{attribute.DisplayName} called");
          m.Invoke(inst, null);
        }
      }
    }

    static IEnumerable<Type> GetTypesWithAttribute<T>(Assembly assembly) {
      foreach(Type type in assembly.GetTypes()) {
        if (type.GetCustomAttributes(typeof(T), true).Length > 0) {
            yield return type;
        }
      }
    }

    static void WriteLine(string value)
    {
      Console.WriteLine(value);
    }
  }

  public static class Trace
  {
    public static void WriteLine(string value)
    {
      Console.WriteLine(value);
    }

    public static void WriteLine(object value)
    {
      Console.WriteLine(value);
    }
  }
}