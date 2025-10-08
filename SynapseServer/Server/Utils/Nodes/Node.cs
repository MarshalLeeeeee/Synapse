
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

    #region REGION_STREAM

    public override bool ShouldSerializeContent(string proxyId)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) return true;
        else if (nodeSyncType == NodeSyncConst.SyncOwn)
        {
            string? playerId = Game.Instance?.GetManager<EntityManager>()?.GetPlayerIdByProxyId(proxyId);
            string rootId = GetRootId();
            return playerId == rootId;
        }
        else return false;
    }

    #endregion

    #region REGION_SYNC

    protected void SyncOwn(string methodName, params Node[] args)
    {
        string rootId = GetRootId();
        string? accountId = Game.Instance?.GetManager<EntityManager>()?.GetAccountByPlayerId(rootId);
        if (accountId == null) return;

        string? proxyId = Game.Instance?.GetManager<AccountManager>()?.GetProxyId(accountId);
        if (proxyId == null) return;

        Game.Instance.CallRpc(proxyId, methodName, GetFullId(), args);
    }

    protected void SyncAll(string methodName, params Node[] args)
    {
       AccountManager? accountManager = Game.Instance.GetManager<AccountManager>();
        if (accountManager == null) return;

        List<string> proxyIds = accountManager.GetProxyIds();
        foreach (string proxyId in proxyIds)
        {
            Game.Instance.CallRpc(proxyId, methodName, GetFullId(), args);
        }
    }

    #endregion
}
