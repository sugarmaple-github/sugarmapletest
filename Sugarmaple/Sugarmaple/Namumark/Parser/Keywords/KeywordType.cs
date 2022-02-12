namespace Sugarmaple.Namumark.Parser
{
  internal enum KeywordType: byte
  {
    None,
    Intact,
    IntactAndMarkInside,
    OpenLifo,
    OpenCloseFifo,
    Close,
    Fail,
    AccumulateStart,
    AccumulateAsList,
    AccumulateAsLine,
    //누적 개행 구문
  }
}