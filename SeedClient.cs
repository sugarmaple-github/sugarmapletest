using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Sugarmaple.Web;

namespace Sugarmaple
{
  public class SeedClient
  {
    private static readonly JsonSerializerOptions options =
      new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    
    private readonly HttpClient client = new HttpClient();
    private string headerToken;
  
    private string? lastDocument = null;
    private ViewResponse? lastView = null;
    public SeedWiki Wiki { get; }

    public SeedClient(SeedWiki wiki, string apiToken)
    {
      Wiki = wiki;
      UpdateApiToken(apiToken);
      //client.DefaultRequestHeaders.
    }

    private const string InvalidApiMessage = "{\"status\":\"권한이 부족합니다.\"}";

    #region Public Method
    public async Task<string?> ViewAsStringAsync(string document)
    {
      if (document.Length == 0)
        throw new ArgumentException("The name of document can't be empty string", nameof(document));
      try
      {
        var response = await GetViewResponseAsync(document);

        if(!response.Exists)
          return null;
        return response.Text;
      }
      catch (AggregateException e)
      {
        throw new InvalidApiTokenException(this, true);
      }
      return null;
    }

    public async Task<EditResponse> EditAsync(string text, string log)
    {
      return await GetEditResponseAsync(text, log);
    }

    public async Task<RedirectResponse> RedirectAsync(string document)
    {
      throw new Exception("");
    }

    public void UpdateApiToken(string apiToken)
    {
      if (!HasOnlyAscii(apiToken))
        throw new ArgumentException($"{nameof(apiToken)} must contain only ASCII characters.", nameof(apiToken));

      var pastToken = headerToken;
      headerToken = $"Bearer {apiToken}";
      if (!IsApiValid())
      {
        headerToken = pastToken;
        throw new InvalidApiTokenException(this);
      }
    }

    public bool IsApiValid()
    {
      var content = GetEditContent(" ");
      var output = content.ReadAsStringAsync().Result;
      return output != InvalidApiMessage;
    }

    internal void DoTest()
    {
      var s = Console.ReadLine();
      var resp = GetViewResponseAsync(s).Result;
      Console.WriteLine(resp);
      var editResp = GetEditResponseAsync("아무거나", "되나").Result;
      Console.WriteLine(editResp);
      Console.ReadLine();
    }
    #endregion

    private async Task<ViewResponse> GetViewResponseAsync(string document)
    {
      var content = GetEditContent(document);
      var output = await ReadFromJsonAsync<ViewResponse>(content);
      lastView = output;
      lastDocument = document;
      return output;
    }

    private async Task<string> GetViewResponseAsStringAsync(string document)
    {
      var content = GetEditContent(document);
      var output = await content.ReadAsStringAsync();
      if(output == InvalidApiMessage)
        throw new InvalidApiTokenException(this, true);
      return output;
    }

    private async Task<EditResponse> GetEditResponseAsync(string text, string log)
    {
      var parameter = new EditParameter(text, log, lastView.Token);
      var content = PostEditContent(lastDocument, ref parameter);
      var output = await ReadFromJsonAsync<EditResponse>(content);
      Console.WriteLine($"Status = {output.Status}");
      return output;
    }

    private async Task<RedirectResponse> GetRedirectResponse(string document, RedirectParameter parameter)
    {
      throw new NotImplementedException();
    }

    private static bool HasOnlyAscii(string value)
    {
      foreach(var c in value)
        if(c > sbyte.MaxValue)
          return false;
      return true;
    }

    private Task<T> ReadFromJsonAsync<T>(HttpContent content)
    {
      try
      {
        var output = content.ReadFromJsonAsync<T>(options);
        return output;
      }
      catch (Exception e)
      {
        Console.WriteLine("Json외 값이 반환되었습니다.");
        var stringOutput = content.ReadAsStringAsync().Result;
        Console.WriteLine(stringOutput);
        Console.ReadLine();
      }
      return null;
    }

    private HttpContent GetEditContent(string document)
    {
      using (var request = CreateRequest(HttpMethod.Get, $"api/edit/{document}"))
      {
          var response = client.SendAsync(request).Result;
          return response.Content;
      }
    }

    private HttpContent PostEditContent(string document, ref EditParameter data)
    {
      using (var request = CreateRequest(HttpMethod.Post, $"api/edit/{document}"))
      {
        var headerValue = MediaTypeHeaderValue.Parse("application/json"); 
        var jsonContent = JsonContent.Create<EditParameter>(data, headerValue, options);
        request.Content = jsonContent;
        
        var response = client.SendAsync(request).Result;
        return response.Content;
      }
    }

    private HttpContent GetRedirectContent(string document, RedirectParameter parameter)
    {
      throw new NotImplementedException();
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string address)
    {
      var request = new HttpRequestMessage(method, $"{Wiki.BaseAddress}/{address}");
      request.Headers.TryAddWithoutValidation("Authorization", headerToken); 
      return request;
    }
  }
}