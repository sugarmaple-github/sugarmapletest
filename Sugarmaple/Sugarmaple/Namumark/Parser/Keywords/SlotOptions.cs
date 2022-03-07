using System;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  [Flags]
  internal enum SlotOptions: byte
  {
    None,
    Markable    = 1 << 0,
    Optional    = 1 << 1,
    SingleLine  = 1 << 2,
  }
}