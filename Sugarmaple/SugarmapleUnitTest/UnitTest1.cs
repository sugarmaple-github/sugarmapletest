#nullable enable
//#define VS
using System.IO;
using System.Collections.Generic;
using System.Text;
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
      TestMethodRegex();
      WriteTokens();
    }
    
    public void WriteTokens()
    {
      var text = File.ReadAllText(@$"{Directory.GetCurrentDirectory()}/TestInput/doc.txt");
      var tokens = context.GetTokens(text);
      var count = 20;
      foreach (var token in tokens)
      {
        Trace.WriteLine(tokenToString(token));
        if (count-- <= 0)
          break;
      }
    }

    static string tokenToString(NamuToken token)
    {
      var builder = new StringBuilder();
      builder.Append(@$"{{
        SyntaxCode: {token.SyntaxCode}
                     }}");
      foreach(var o in token.Argument.Select(o => o.Raw))
      {
        builder.AppendLine($"{o}");
      }
      builder.Append('}');
      
      return builder.ToString();
    }

    NamumarkContext context = NamumarkContext.Instance;
                     
    //[TestMethod]
    public void TestMethodRegex()
    {
      var regContext = context.GetFieldValue<NamumarkRegContext, NamumarkContext>("_regContext");
      var regexRaw = regContext.GetFieldValue<Regex, NamumarkRegContext>("_regex");
      Trace.WriteLine(regexRaw);
      var indice = regContext.GetFieldValue<List<int>, NamumarkRegContext>("_namedGroupIndice");
      foreach(var index in indice)
      {
        Trace.WriteLine(index);
      }
    }

    /*internal void DoTest()
    {
      var json = JsonDocument.Parse("{\"namespaces\":[{\"namespace\":\"사용자\",\"count\":1}],\"backlinks\":[{\"document\":\"사용자:koreapyj/test1\",\"flags\":\"link\"}],\"from\":null,\"until\":null}");
      var document = Console.ReadLine()!;
      string query = $"";
      var uri = CreateUri().AddPath("backlink").AddPath(document);
      var doc = GetJson(uri);
      var root = doc.RootElement;
      var backlinks = root.GetProperty("backlinks").EnumerateArray();
      foreach (var l in backlinks)
      {
        Console.WriteLine(l);
      }
      var r = new BacklinkResponse(doc);
      foreach (var b in r.Backlinks)
      {
        Console.WriteLine(b);
      }
      foreach(var n in r.Namespaces)
        Console.WriteLine($"이름공간: {n}");
      //Console.WriteLine(r.Backlinks.Count());
      //var output = ReadFromJson<BacklinkResponse>(content).Result;
      //Console.WriteLine(output);
      //var editResp = GetEditResponseAsync("아무거나", "되나").Result;
      //Console.WriteLine(editResp);
      Console.ReadLine();
    }*/
  }
}
