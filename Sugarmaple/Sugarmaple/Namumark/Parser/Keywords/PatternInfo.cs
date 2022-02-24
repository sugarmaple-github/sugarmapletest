namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class PatternInfo
  {
    public string Raw { get; }
    public int GroupNum { get; }
    public bool IsAccumulating { get; }

    public int MarkableGroup { get; }
    
    public PatternInfo(string raw, int groupNum, bool isAccumulating, int markableGroup)
    {
      Raw = raw;
      GroupNum = groupNum;
      IsAccumulating = isAccumulating;
      MarkableGroup = markableGroup;
    }
  }
}