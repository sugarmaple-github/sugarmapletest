#nullable enable
using System;

namespace SugarmapleUnitTest
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
  [Serializable]
  internal class TestClassAttribute: Attribute
  {
    public TestMethodAttribute GetTestMethodAttribute()
    {
      throw new NotImplementedException();
      //미구현
      return null;
    }
  }

  [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
  internal class TestMethodAttribute: Attribute
  {
    public TestMethodAttribute(string displayName = "Test1")
    {
      DisplayName = displayName;
    }

    public string DisplayName { get; }

    public void Execute()
    {
      
    }
  }
}