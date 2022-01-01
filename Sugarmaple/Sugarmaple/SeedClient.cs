using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sugarmaple.Web;

namespace Sugarmaple
{
  public class SeedClient
  {
    private static readonly JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private static readonly MediaTypeHeaderValue contentType = MediaTypeHeaderValue.Parse("application/json");

    private readonly HttpClient client = new HttpClient();
  
    private string? lastDocument;
    private string? editToken;
    public SeedWiki Wiki { get; }

    public SeedClient(SeedWiki wiki, string apiToken)
    {
      Wiki = wiki;
      client.BaseAddress = wiki.Uri;
      UpdateApiToken(apiToken);
    }

    private const string InvalidApiMessage = "{\"status\":\"권한이 부족합니다.\"}";

    #region Public Method
    public string? ViewAsString(string document)
    {
      if (string.IsNullOrWhiteSpace(document))
        throw new ArgumentException("The name of document can't be null or white space.", nameof(document));
      try
      {
        using (var response = GetViewResponse(document))
        {
          if(!response.Exists)
            return null;
          return response.Text;
        }
      }
      catch (AggregateException e)
      {
        throw new InvalidApiTokenException(this, true);
      }
    }

    public Task<string?> ViewAsStringAsync(string document)
    {
      return new Task<string?>(() => ViewAsString(document));
    }

    public Task<SeedDocumentText> View(string doucment)
    {
      throw new NotImplementedException();
    } 

    public EditResponse Edit(string text, string log)
    {
      return GetEditResponse(text, log);
    }

    public Task<EditResponse> EditAsync(string text, string log)
    {
      return new Task<EditResponse>(() => Edit(text, log));
    }

    public BacklinkResponse GetBacklink(string document, string @from = "ACL", string @namespace = "문서", int flag = 0)
    {
      return GetBacklinkResponse(document, @from, @namespace, flag);
    }

    public Task<BacklinkResponse> GetBacklinkAsync(string document, string @from = "", string @namespace = "", int flag = 0)
    {
      return new Task<BacklinkResponse>(() => GetBacklinkResponse(document, @from, @namespace, flag));
    }

    public void UpdateApiToken(string apiToken)
    {
      if (!HasOnlyAscii(apiToken))
        throw new ArgumentException($"{nameof(apiToken)} must contain only ASCII characters.", nameof(apiToken));

      var pastAuth = client.DefaultRequestHeaders.Authorization;
      client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {apiToken}");
      if (!IsApiValid())
      {
        client.DefaultRequestHeaders.Authorization = pastAuth;
        throw new InvalidApiTokenException(this);
      }
    }

    public bool IsApiValid()
    {
      var uri = CreateUri("edit", " ");
      var content = client.GetAsync(uri).Result.Content;
      var output = content.ReadAsStringAsync().Result;
      return output != InvalidApiMessage;
    }
    #endregion

    #region Web Struct Builder
    private ViewResponse GetViewResponse(string document)
    {
      var uri = CreateUri("edit", document);
      var jsonDoc = GetJsonDocument(uri);
      var output = new ViewResponse(jsonDoc);
      editToken = output.Token;
      lastDocument = document;
      return output;
    }

    private EditResponse GetEditResponse(string text, string log)
    {
      if (lastDocument == null)
        throw new InvalidOperationException("To edit, at least one document should be viewed.");
      var uri = CreateUri("edit", lastDocument!);
      var parameter = new EditParameter(text, log, editToken!);
      var jsonDoc = GetJsonDocumentByPost<EditParameter>(uri, ref parameter);
      var output = new EditResponse(jsonDoc);
      return output;
    }



    private BacklinkResponse GetBacklinkResponse(string document, string @from, string @namespace, int flag)
    {
      var uri = CreateUri("backlink", document)
        .AddQuery(nameof(@from), @from)
        .AddQuery(nameof(@namespace), @namespace);
      var jsonDoc = GetJsonDocument(uri);
      uri.Dispose();
      var output = new BacklinkResponse(jsonDoc);
      return output;
    }

    private BacklinkResponse GetBacklinkResponseByUntil(string document, string until, string @namespace, int flag)
    {
      var uri = CreateUri("backlink", document)
        .AddQuery(nameof(until), until)
        .AddQuery(nameof(@namespace), @namespace)
        .AddQuery(nameof(flag), flag);
      var jsonDoc = GetJsonDocument(uri);
      uri.Dispose();
      var output = new BacklinkResponse(jsonDoc);
      return output;
    }
    #endregion

    #region Json Creator
    private JsonDocument GetJsonDocument(RelativeUri uri)
    {
      var stream = client.GetAsync(uri).Result.Content.ReadAsStream();
      //StreamReader reader = new StreamReader(stream);
      //string text = reader.ReadToEnd();
      //Console.WriteLine($"Inner Test: {text}");
      //Console.ReadLine();
      //if(output == InvalidApiMessage)
      //  throw new InvalidApiTokenException(this, true);
      return JsonDocument.Parse(stream);
    }

    private JsonDocument GetJsonDocumentByPost<T>(RelativeUri uri, ref T data)
    {
      var jsonContent = JsonContent.Create<T>(data, contentType, options);
      var stream = client.PostAsync(uri, jsonContent).Result.Content.ReadAsStream();
      return JsonDocument.Parse(stream);
    }

    #endregion

    #region Utility
    private static RelativeUri CreateUri() => RelativeUri.Create("api");
    private static RelativeUri CreateUri(string path, string document) => CreateUri().AddPath(path).AddPath(document);

    private static bool HasOnlyAscii(string value)
    {
      foreach(var c in value)
        if(c > sbyte.MaxValue)
          return false;
      return true;
    }
    #endregion

    #region Test Method
    internal void DoTest()
    {
      var json = JsonDocument.Parse("{\"namespaces\":[{\"namespace\":\"사용자\",\"count\":1}],\"backlinks\":[{\"document\":\"사용자:koreapyj/test1\",\"flags\":\"link\"}],\"from\":null,\"until\":null}");
      var document = Console.ReadLine()!;
      string query = $"";
      var uri = CreateUri().AddPath("backlink").AddPath(document);
      var doc = GetJsonDocument(uri);
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
    }
    #endregion
  }
}