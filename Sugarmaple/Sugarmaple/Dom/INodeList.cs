//copy from https://github.com/AngleSharp/AngleSharp/blob/devel/src/AngleSharp/Dom/INodeList.cs

using System.Collections.Generic;
using Sugarmaple.Attributes;

namespace Sugarmaple.Dom
{
    /// <summary>
    /// NodeList objects are collections of nodes.
    /// </summary>
    [DomName("NodeList")]
    public interface INodeList : IEnumerable<INode>//, IWikiMarkupFormattable
    {
        /// <summary>
        /// Returns an item in the list by its index, or throws an exception.
        /// </summary>
        /// <param name="index">The 0-based index.</param>
        /// <returns>The element at the given index.</returns>
        [DomName("item")]
        //[DomAccessor(Accessors.Getter)]
        INode this[int index] { get; }

        /// <summary>
        /// Gets the number of nodes in the NodeList.
        /// </summary>
        [DomName("length")]
        int Length { get; }
    }
}