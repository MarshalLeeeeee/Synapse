
public class Node : NodeCommon
{
    /// <summary>
    /// the sync type of the node
    /// </summary>
    public int nodeSyncType { get; protected set; } = NodeSyncConst.SyncAll;

    protected Node() : base() { }

    public void SetNodeSyncType(int nodeSyncType_)
    {
        nodeSyncType = nodeSyncType_;
    }

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

    #region REGION_STREAM

    public override bool ShouldSerializeContent(string proxyId)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) return true;
        else if (nodeSyncType == NodeSyncConst.SyncOwn)
        {
            string? playerId = Game.Instance.GetManager<EntityManager>()?.GetPlayerIdByProxyId(proxyId);
            string rootId = GetRootId();
            return playerId == rootId;
        }
        else return false;
    }

    #endregion
}
