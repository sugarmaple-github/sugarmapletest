using System;
using System.Text.Json;

namespace Sugarmaple.Web
{
  internal struct ViewResponse
  {
    public string Text { get; }
    public bool Exists { get; }
    public string Token { get; }
  }
}