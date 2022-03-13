namespace Sugarmaple.Namumark.Parser
{
  internal enum CommandType: byte
  {
    None,
    Intact,
    OpenLifo,
    OpenCloseFifo,
    Close,
    Fail,

    //리스트를 시작하는 타입으로 하려했으나, 필요성이 부족하다고 판단해 보류.
    //AccumulateStart,

    //기본적으로 같은 태그의 리스트가 앞에 등장했다면, 그 구문에 리스트로서 누적됩니다.
    //그렇지 않다면, 리스트를 시작합니다.
    AccumulateAsList,

    //리스트가 앞에 등장했다면, 개행으로서 누적됩니다.
    //그렇지 않다면, 들여쓰기를 시작합니다.
    AccumulateAsLine,
    Complex,
    //해당 그룹에서 해당 문법 맥락에 따라 토크나이징을 다시 진행합니다.
    Context,    
  }
}