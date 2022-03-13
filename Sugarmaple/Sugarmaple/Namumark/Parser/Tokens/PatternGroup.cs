using Sugarmaple.Namumark.Parser.Keywords;

namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class PatternGroup: PatternCapture
  {
    public TokenCommand Command { get; }

    public PatternGroup(string text, int index, int length, TokenCommand? command): base(text, index, length)
    {
      Command = command ?? TokenCommand.Empty;
    }
  }
}