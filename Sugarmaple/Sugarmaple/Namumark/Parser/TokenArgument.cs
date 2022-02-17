namespace Sugarmaple.Namumark.Parser
{ 
  internal struct TokenArgument
  {
    public string? Tag { get; }
    public string? Parameter { get; }
    public int Level { get; }

    public TokenArgument(string? tag, string? parameter, int level)
    {
      Tag = tag;
      Parameter = parameter;
      Level = level;
    }
  }
}