using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sugarmaple.Namumark.Parser;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SugarmapleUnitTest
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      var config = Namumark.Config;
      foreach (var c in config)
      {
        var s = c.RegexRaw;
        //Assert.IsNotNull(s);
        //Assert.AreNotSame(s, "");
        Trace.WriteLine(s);
      }
    }

    [TestMethod]
    public void TestMethodRegex()
    {
      var r = new Regex(@"(?<32>A)(?<12>B)()()\k<2>");
    }
  }
}
