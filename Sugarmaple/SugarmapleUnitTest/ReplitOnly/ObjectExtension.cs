#nullable disable
using System;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace SugarmapleUnitTest
{
  //https://stackoverflow.com/questions/10789644/testing-a-private-field-using-mstest
  [ExcludeFromCodeCoverage]
  internal static class ObjectExtension
  {
    public static T GetFieldValue<T>(this object sut, string name)
    {
        var field = sut
            .GetType()
            .GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        return (T)field?.GetValue(sut);
    }

    public static outT GetFieldValue<outT, baseT>(this object sut, string name)
    {
        var field = typeof(baseT)
            .GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        return (outT)field?.GetValue(sut);
    }

    public static T GetPropertyValue<T>(this object sut, string name)
    {
        var field = sut
            .GetType()
            .GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        return (T)field?.GetValue(sut);
    }
  }
}