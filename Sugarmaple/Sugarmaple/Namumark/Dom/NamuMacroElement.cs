using Sugarmaple.Dom;

namespace Sugarmaple.Namumark.Dom
{
  public class NamuMacroElement : NamuElement
  {
    internal NamuMacroElement(DomBasicArgument argument, string macroName, string? parameter): base(argument)
    {
        MacroName = macroName;
        Parameter = parameter;
    }

    public string MacroName;
    public NamuMacroType MacroType;//구현 예정
    public string? Parameter;
  }
}