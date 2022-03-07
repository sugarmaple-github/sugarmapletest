using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Sugarmaple.Web;

namespace Sugarmaple.Web
{
  internal static class HttpClientExtension
  {
    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions 
      { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private static readonly JsonSerializerOptions _deserializerOptions = new JsonSerializerOptions
      { PropertyNameCaseInsensitive = true };
    private static readonly MediaTypeHeaderValue _contentType = MediaTypeHeaderValue.Parse("application/json");

    public static JsonDocument JsonByGet(this HttpClient client, RelativeUri uri)
    {
      var stream = client.GetAsync(uri).Result.Content.ReadAsStream();
      return JsonDocument.Parse(stream);
    }

    public static TOut DataByGet<TOut>(this HttpClient client, RelativeUri uri)
    {
      var stream = client.GetAsync(uri).Result.Content.ReadAsStream();
      uri.Dispose();
      return JsonSerializer.DeserializeAsync<TOut>(stream, _deserializerOptions).Result;
    }
    
    public static JsonDocument JsonByPost<T>(this HttpClient client, RelativeUri uri, T data)
    {
      var jsonContent = JsonContent.Create<T>(data, _contentType, _serializerOptions);
      var stream = client.PostAsync(uri, jsonContent).Result.Content.ReadAsStream();
      return JsonDocument.Parse(stream);
    }

    public static TOut DataByPost<TOut, T>(this HttpClient client, RelativeUri uri, T data)
    {
      var jsonContent = JsonContent.Create<T>(data, _contentType, _serializerOptions);
      var stream = client.PostAsync(uri, jsonContent).Result.Content.ReadAsStream();
      uri.Dispose();
      return JsonSerializer.DeserializeAsync<TOut>(stream, _deserializerOptions).Result;
    }

    //public static T Deserialize<T>(this JsonDocument doc) 
    //{
    //  var output = doc.Deserialize<T>(_deserializerOptions);
    //  doc.Dispose();
    //  return output;
    //}
  }
}