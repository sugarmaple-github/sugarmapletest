using System;
using System.Text;
using Sugarmaple.Text;

namespace Sugarmaple.Web
{
  internal struct RelativeUri: IDisposable
  {
    private readonly StringBuilder _builder;
    private bool isQuery;

    public RelativeUri(string basePath)
    {
      _builder = StringBuilderPool.Obtain().Append(basePath);
      isQuery = false;
    }

    public static RelativeUri Create(string basePath) => new RelativeUri(basePath);

    public RelativeUri AddPath(string path) 
    {
      _builder.Append('/').Append(path);
      return this;
    }

    public RelativeUri AddQuery(string name, int value) => AddQuery(name, value.ToString());
    public RelativeUri AddQuery(string name, string value)
    {
      isQuery = true;
      _builder
        .Append(isQuery ? '&' : '?')
        .Append(name)
        .Append('=')
        .Append(value);
      return this;
    }

    public override string ToString()
    {
      return _builder.ToString();
    }

    public void Dispose()
    {
      _builder.ToPool();
    }

    public static implicit operator string(RelativeUri u) => u.ToString();
  }
}