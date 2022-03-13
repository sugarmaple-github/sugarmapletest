using System;

namespace Sugarmaple.Web
{
  public class InvalidApiTokenException: Exception
  {
    internal InvalidApiTokenException(SeedClient client): this($"{client}'s api token is not valid.")
    {

    }

    internal InvalidApiTokenException(SeedClient client, bool expired = false): this(expired ? ExpiredMessage(client) : InvalidEntryMessage(client))
    {

    }

    internal InvalidApiTokenException(string message, Exception inner)
        : base(message, inner)
    {

    }

    internal InvalidApiTokenException(string? message): base(message)
    {

    }

    private static string InvalidEntryMessage(SeedClient client) => $"{client}'s api token is not valid.";
    private static string ExpiredMessage(SeedClient client) => $"{client}'s api token is expired.";
  }
}