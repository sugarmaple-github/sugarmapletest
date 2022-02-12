using System;

namespace Sugarmaple.Web
{
  public class InvalidApiTokenException: Exception
  {
    public InvalidApiTokenException(SeedClient client): this($"{client}'s api token is not valid.")
    {

    }

    public InvalidApiTokenException(SeedClient client, bool expired = false): this(expired ? ExpiredMessage(client) : InvalidEntryMessage(client))
    {

    }

    public InvalidApiTokenException(string? message): base(message)
    {

    }

    private static string InvalidEntryMessage(SeedClient client) => $"{client}'s api token is not valid.";
    private static string ExpiredMessage(SeedClient client) => $"{client}'s api token is expired.";
  }
}