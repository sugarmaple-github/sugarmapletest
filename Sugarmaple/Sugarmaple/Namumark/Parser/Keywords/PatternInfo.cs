using Sugarmaple.Text;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  internal class PatternInfo: IRegexInfo
  {
    private readonly string _regexSkeleton;
    private readonly int[] _backRefIndice;

    public int GroupNum { get; }

    string IRegexInfo.Raw => _regexSkeleton;
    //if baseIndex + index > 10 invalid
    public string BuildRegex(int baseIndex)
    {
      var builder = StringBuilderPool.Obtain();
      var preIndex = 0;
      foreach(var index in _backRefIndice)
      {
        builder.Append(_regexSkeleton, preIndex, index - preIndex);
        builder.Append(baseIndex + _regexSkeleton[index] - '0');
        preIndex = index;
      }
      builder.Append(_regexSkeleton, preIndex, _regexSkeleton.Length - preIndex);
      var result = builder.ToString();
      builder.ToPool();
      return result;
    }

    //obsolete
    public bool IsAccumulating { get; }//?
    
    public PatternInfo(string raw, int groupNum, IEnumerable<int> backRefIndice, bool isAccumulating = false)
    {
      _regexSkeleton = raw;
      _backRefIndice = backRefIndice;
      GroupNum = groupNum;
      IsAccumulating = isAccumulating;
    }
  }
}