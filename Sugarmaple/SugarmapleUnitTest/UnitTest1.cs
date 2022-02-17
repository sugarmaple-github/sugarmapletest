#nullable enable
//#define VS
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Sugarmaple.Namumark.Parser;
#if VS

using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
#endif

namespace SugarmapleUnitTest
{
  [TestClass]
  public class UnitTest1
  {
    NamuTokenizer namumark = NamuTokenizer.Instance;

    //[TestMethod]
    public void TestMethod1()
    {
      namumark.GetFieldValue<List<int>, Tokenizer>("namedGroupIndice").ForEach(o => Trace.WriteLine(o));
      //var text = File.ReadAllText(@$"{Directory.GetCurrentDirectory()}/TestInput/doc.txt");
      //foreac
    }
    
    //[TestMethod]
    public void TokenTest()
    {
      var text = File.ReadAllText(@$"{Directory.GetCurrentDirectory()}/TestInput/doc.txt");
      var count = 3;
      foreach(var token in namumark.GetTokens(text))
      {
        Trace.WriteLine(tokenToString(token));
        count--;
        if(count == 0)
          return;
      }
    }

    static string tokenToString(Token token)
    {
      return @$"{{
        SyntaxCode: {token.SyntaxCode}
        Argument:
          Tag: {token.Argument.Tag}
          Parameter: {token.Argument.Parameter}
          Level: {token.Argument.Level}
}}";
    }

    [TestMethod]
    public void TestMethodRegex()
    {
      var regexRaw = namumark.GetFieldValue<Regex, Tokenizer>("_regex");
      Trace.WriteLine(regexRaw);
    }
  }

  
}
