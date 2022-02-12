using Sugarmaple.Dom;

namespace Sugarmaple.Namumark.Dom
{
  public class NamuHeadingElement : NamuElement
  {
    public int Level { get; }
    public bool Hidden { get; }

    internal NamuHeadingElement(DomBasicArgument argument, int level = 1, bool hidden = false): base(argument)
    {
        Level = level;
        Hidden = hidden;
    }
  }
}