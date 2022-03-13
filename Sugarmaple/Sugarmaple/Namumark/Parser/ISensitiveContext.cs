namespace Sugarmaple.Namumark.Parser
{
  internal interface ISensitiveContext<T> where T: StringRange
  {
    T? GetToken();
  }
}