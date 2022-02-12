namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class PatternInfo
  {
    public string Raw { get; }
    public int GroupNum { get; }
    public bool IsAccumulating { get; }
    private PatternGroup[] groups;
    
    public PatternInfo(string raw, int groupNum, bool isAccumulating, PatternGroup[] groups)
    {
      Raw = raw;
      GroupNum = groupNum;
      IsAccumulating = isAccumulating;
      this.groups = groups;
    }

    public TokenArgument CreateArgument(string[] captures)
    {
      string? tag = null;
      string? parameter = null;
      int level = 0;
      for(int i = 0; i < groups.Length; i++)
      {
        switch (groups[i])
        {
          case PatternGroup.Tag:
            tag = captures[i];
            break;
          case PatternGroup.Parameter:
            parameter = captures[i];
            break;
          case PatternGroup.Level:
            level = int.Parse(captures[i]);
            break;
        }
      }
      return new TokenArgument(tag, parameter, level);
    }
  }
}