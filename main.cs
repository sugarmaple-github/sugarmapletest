using System;
using Sugarmaple;
using Sugarmaple.Web;
using Sugarmaple.Namumark.Parser;

class Program {
  static SeedClient client;

  public static void Main (string[] args) {
    LoginByApi();
    client.DoTest();
    Console.ReadLine();
  }

  private static void LoginByApi()
  {
    Console.WriteLine("Please Write Api:");
    var api = Console.ReadLine();
    client = new SeedClient(SeedWiki.TheSeedWiki, api);
    Console.WriteLine("Checked");
  }

  private static void ViewAsString(string document)
  {
    var task = client.ViewAsStringAsync(document);
    var result = task.Result;
    Console.WriteLine(result);
  }

  /*private static void View(string document)
  {
    var task = client.ViewAsync(document);
    var result = task.Result;
    Console.WriteLine(result);
  }*/

  private static void Edit(string text, string log)
  {
    var task = client.EditAsync(text, log);
    var result = task.Result;
    Console.WriteLine(result);
  }
}