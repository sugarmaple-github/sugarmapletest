using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sugarmaple.Web;
using Sugarmaple.Text;

namespace Sugarmaple
{
  public class SeedClient
  {
    private readonly HttpClient _client = new HttpClient();
  
    private string? _lastDocument;
    private string? _editToken = null;
    public SeedWiki Wiki { get; }

    public SeedClient(SeedWiki wiki, string apiToken)
    {
      Wiki = wiki;
      _client.BaseAddress = wiki.Uri;
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
        var response = _client.DataByGet<ViewResponse>(CreateUri("edit", document));
        _editToken = response.Token;
        _lastDocument = document;
        return response.Exists ? response.Text : null;
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

    //public Task<Document> View(string doucment)
    //{
    //  throw new NotImplementedException();
    //} 

    public Task<EditResponse> EditAsync(string text, string log)
    {
      return new Task<EditResponse>(() => Edit(text, log));
    }

    public EditResponse Edit(string text, string log)
    {
      if (_lastDocument == null)
        throw new InvalidOperationException("To edit, at least one document should be viewed.");
      return _client.DataByPost<EditResponse, EditParameter>(CreateUri("edit", _lastDocument),
        new EditParameter { Text = text, Log = log, Token = _editToken! });
    }

    public Task<BacklinkResponse> GetBacklinkAsync(string document, string @from = "", string @namespace = "", int flag = 0)
    {
      return new Task<BacklinkResponse>(() => GetBacklink(document, @from, @namespace, flag));
    }

    public BacklinkResponse GetBacklink(string document, string @from = "ACL", string @namespace = "문서", int flag = 0)
    {
      return _client.DataByGet<BacklinkResponse>(
        CreateUri("backlink", document)
          .AddQuery(nameof(@from), @from)
          .AddQuery(nameof(@namespace), @namespace));
    }

    public BacklinkResponse GetBacklinkResponseByUntil(string document, string until, string @namespace, int flag)
    {
      return _client.DataByGet<BacklinkResponse>(
        CreateUri("backlink", document)
          .AddQuery(nameof(until), until)
          .AddQuery(nameof(@namespace), @namespace)
          .AddQuery(nameof(flag), flag));
    }

    public void UpdateApiToken(string apiToken)
    {
      if (!apiToken.HasOnlyAscii())
        throw new ArgumentException($"{nameof(apiToken)} must contain only ASCII characters.", nameof(apiToken));

      var pastAuth = _client.DefaultRequestHeaders.Authorization;
      _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {apiToken}");
      if (!IsApiValid())
      {
        _client.DefaultRequestHeaders.Authorization = pastAuth;
        throw new InvalidApiTokenException(this);
      }
    }

    public bool IsApiValid()
    {
      var uri = CreateUri("edit", " ");
      var content = _client.GetAsync(uri).Result.Content;
      var output = content.ReadAsStringAsync().Result;
      return output != InvalidApiMessage;
    }
    #endregion

    private struct EditParameter
    {
      public string Text { get; init; }
      public string Log { get; init; }
      public string Token { get; init; }
    }

    private struct BacklinkParameter
    {
      public string Namespace { get; init; }
      public string From { get; init; }
      public string Until { get; init; }
    }

    #region Utility
    private static RelativeUri CreateUri() => RelativeUri.Create("api");
    private static RelativeUri CreateUri(string path, string document) => CreateUri().AddPath(path).AddPath(document);
    #endregion
  }
}