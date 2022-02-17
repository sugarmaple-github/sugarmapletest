//copy from https://github.com/AngleSharp/AngleSharp/blob/devel/src/AngleSharp/Dom/NodeExtensions.cs

namespace Sugarmaple.Dom
{
  public static class NodeExtensions
  {
    public static bool IsDescendantOf(this INode node, INode parent)
    {
      while (node.Parent != null)
      {
        if (object.ReferenceEquals(node.Parent, parent))
        {
          return true;
        }

        node = node.Parent;
      }

      return false;
    }
  }
}