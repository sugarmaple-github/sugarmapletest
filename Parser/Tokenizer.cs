#if false
namespace Sugarmaple.Namumark.Parser
{
  internal abstract class Tokenizer
  {
    #region Fields
    private readonly StringTape source = new StringTape();

    private StringBuilder buffer = new StringBuilder();
    #endregion

    public Tokenizer(StringTape tape)
    {

    }

    protected char Previous

    protected char GetNext()
    {

    }

    
  }

  internal enum NamuParseMode
  {
    ParseData,
    RawText,
  }
}
#endif