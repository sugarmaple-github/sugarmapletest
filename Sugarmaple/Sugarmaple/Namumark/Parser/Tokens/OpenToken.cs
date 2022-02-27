using Sugarmaple.Namumark.Parser.Keywords;

namespace Sugarmaple.Namumark.Parser.Tokens
{
  internal class OpenToken: Token
  {
    public OpenTokenCommand Command { get; }

    public OpenToken(int index, int length, OpenTokenCommand command): base(index, length, TokenizeOperation.None)
    {
      Command = command;
    }
  }
}