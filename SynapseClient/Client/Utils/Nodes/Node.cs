
public class Node : NodeCommon
{
    protected Node(string id_ = "") : base(id_) { }

    #region REGION_IDENTIFICATION

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
