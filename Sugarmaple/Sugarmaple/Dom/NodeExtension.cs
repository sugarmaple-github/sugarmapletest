namespace Sugarmaple.Dom
{
  public static class NodeExtension
  {
    public static bool IsDescendantOf(this INode parent)
    {
      Node node = this;
      while (node.Parent != null)
      {
        if (Object.ReferenceEquals(Parent, parent))
          return true;

        node = node.Parent;
      }
      return false;
    }
  }
}