#nullable enable
//#define VS
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Sugarmaple.Namumark.Parser;
using Sugarmaple.Namumark.Parser.Tokens;
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
    //[TestMethod]
    public void TestMethod1()
    {
      //namumark.GetFieldValue<List<int>, Tokenizer>("namedGroupIndice").ForEach(o => Trace.WriteLine(o));
      //var text = File.ReadAllText(@$"{Directory.GetCurrentDirectory()}/TestInput/doc.txt");
      //foreac
    }
    
    [TestMethod]
    public void TokenTest()
    {
      var text = File.ReadAllText(@$"{Directory.GetCurrentDirectory()}/TestInput/doc.txt");
      var namumark = Namumark.GetTokenizer(text);
      var startAt = 10;
      var count = 20;
      for(int i = 0; i < startAt && !namumark.IsEnd; i++)
      {
        var token = namumark.GetToken();
      }
      for(int i = startAt; i < count && !namumark.IsEnd; i++)
      {
        Trace.WriteLine(i);
        Trace.WriteLine(namumark.IsEnd);
        var token = namumark.GetToken();
        Trace.WriteLine(tokenToString(token));
      }
    }

    static string tokenToString(ElementToken token)
    {
      var builder = new StringBuilder();
      builder.Append(@$"{{
        SyntaxCode: {token.SyntaxCode}
        Argument: ");
      foreach(var o in token.Argument.Select(o => o.Raw))
      {
        builder.AppendLine($"{o}");
      }
      builder.Append('}');
      
      return builder.ToString();
    }

    //[TestMethod]
    public void TestMethodRegex()
    {
      var context = Namumark.Instance;
      
      var regexRaw = context.GetFieldValue<Regex, Context>("_regex");
      Trace.WriteLine(regexRaw);
    }
  }

  
}
