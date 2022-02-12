using System;

namespace Sugarmaple.Namumark.Parser.Keywords
{
  [Flags]
  internal enum SegmentOptions
  {
    None,
    Markable =    1 << 0,
    Optional =    1 << 1,
    SingleLine =  1 << 2,
    Tag =         1 << 3,
    Parameter =    1 << 4,
    Level =       1 << 5,
  }
}