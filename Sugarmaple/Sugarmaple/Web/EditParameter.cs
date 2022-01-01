namespace Sugarmaple.Web
{
  public struct EditParameter
  {
    public EditParameter(string text, string log, string token)
    {
      Text = text;
      Log = log;
      Token = token;
    }

    public string Text { get; }
    public string Log { get; }
    public string Token { get; }
  }
}