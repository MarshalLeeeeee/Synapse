
[SyncNode(NodeTypeConst.TypeIntKeyDictionary)]
public class IntKeyDictionaryNode : IntKeyDictionaryNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeIntKeyDictionary;

    public IntKeyDictionaryNode(
        params KeyValuePair<int, Node>[] kvps
    ) : base(kvps) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (IntKeyDictionaryNode)Activator.CreateInstance(typeof(IntKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy IntKeyDictionaryNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static IntKeyDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (IntKeyDictionaryNode)Activator.CreateInstance(typeof(IntKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntKeyDictionaryNode.", ex);
        }
    }

    #endregion

    #region REGION_API

    protected override void OnSet(int key, Node value)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("IntKeyDictionaryNode.SetRemote", new IntNode(key), value);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("IntKeyDictionaryNode.SetRemote", new IntNode(key), value);
    }

    protected override void OnAdd(int key, Node value)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("IntKeyDictionaryNode.AddRemote", new IntNode(key), value);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("IntKeyDictionaryNode.AddRemote", new IntNode(key), value);
    }

    protected override void OnRemove(int key)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("IntKeyDictionaryNode.RemoveRemote", new IntNode(key));
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("IntKeyDictionaryNode.RemoveRemote", new IntNode(key));
    }

    protected override void OnClear()
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("IntKeyDictionaryNode.ClearRemote");
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("IntKeyDictionaryNode.ClearRemote");
    }

    #endregion
}
