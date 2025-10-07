
public class Node : NodeCommon
{
    protected Node() : base() { }

    #region REGION_IDENTIFICATION

    /// <summary>
    /// get child node with specific path
    /// </summary>
    /// <param name="path"> string array: path from current node to target child node </param>
    /// <returns> Node instance if target child exists, null otherwise </returns>
    public virtual Node? GetChildWithPath(string[] path)
    {
        if (path.Length == 0) return this;
        return GetChildWithId(path[0])?.GetChildWithPath(path[1..]);
    }

    /// <summary>
    /// Create a deep copy of the node.
    /// </summary>
    /// <returns> Deep copy of the node. </returns>
    public virtual Node Copy()
    {
        throw new InvalidDataException("Failed to copy Node.");
    }

    #endregion
}
