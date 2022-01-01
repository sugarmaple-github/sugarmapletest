using System.Text;
using Sugarmaple.Text;

namespace Sugarmaple.Web
{
  internal struct RelativeUri: System.IDisposable
  {
    private StringBuilder builder;
    private bool isQuery;

    public RelativeUri(string basePath)
    {
      builder = new StringBuilder(basePath);
      isQuery = false;
    }

    public static RelativeUri Create(string basePath) => new RelativeUri(basePath);

    public RelativeUri AddPath(string path) 
    {
      builder.Append('/').Append(path);
      return this;
    }

    public RelativeUri AddQuery(string name, int value) => AddQuery(name, value.ToString());
    public RelativeUri AddQuery(string name, string value)
    {
      isQuery = true;
      builder
        .Append(isQuery ? '&' : '?')
        .Append(name)
        .Append('=')
        .Append(value);
      return this;
    }

    public override string ToString()
    {
      return builder.ToString();
    }

    public void Dispose()
    {
      builder.ToPool();
    }

    public static implicit operator string(RelativeUri u) => u.ToString();
  }
}