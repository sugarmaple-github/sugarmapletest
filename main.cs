using System;
using Sugarmaple;
using Sugarmaple.Web;
using Sugarmaple.Text;
using Sugarmaple.Namumark.Parser;

class Program {
  static SeedClient? client;

  public static void Main (string[] args) {
    LoginByApi();
    ShowOptions();
    while (ProcessChoice(int.Parse(Console.ReadLine()!))) {}
    Console.WriteLine("Program End");
    Console.ReadLine();
  }

  private static bool ProcessChoice(int input)
  {
    switch (input)
    {
      case 0:
        var result0 = client!.ViewAsString(ReadDocument());
        Console.WriteLine(result0);
        break;
      case 1:
        var result1 = client!.Edit(ReadDocumentContent(), ReadLog());
        Console.WriteLine(result1);
        break;
      case 2:
        var result2 = client!.GetBacklink(ReadDocument());
        Console.WriteLine(result2);
        break;
      case 3:
        client!.DoTest();
        break;
      case -1:
        return false;
    }
    return true;
  }

  static void DoTest()
  {
    var backlink = client!.GetBacklink(ReadDocument());
    foreach(var n in backlink.Namespaces)
      Console.WriteLine($"이름공간: {n}");
  }

  private static void ShowOptions()
  {
    var options = new string[] {"View", "Edit", "Backlink", "Special Test"};
    for(int i = 0; i < options.Length; i++)
      Console.WriteLine($"{i}: {options[i]}");
    Console.WriteLine("Write Option Number: ");
  }

  private static void LoginByApi()
  {
    Console.WriteLine("Please Write Api:");
    var api = Console.ReadLine()!;
    client = new SeedClient(SeedWiki.TheSeedWiki, api);
    Console.WriteLine("Checked");
  }

  private static string ReadDocumentContent()
  {
    Console.WriteLine("Please Write Document Content:");
    return Console.ReadLine()!;
  }

  private static string ReadDocument()
  {
    Console.WriteLine("Please Write Document Title:");
    return Console.ReadLine()!;
  }

  private static string ReadLog()
  {
    Console.WriteLine("Please Write Log:");
    return Console.ReadLine()!;
  }
}