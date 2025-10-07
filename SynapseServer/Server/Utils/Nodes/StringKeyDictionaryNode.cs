
[SyncNode(NodeTypeConst.TypeStringKeyDictionary)]
public class StringKeyDictionaryNode : StringKeyDictionaryNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeStringKeyDictionary;

    public StringKeyDictionaryNode(
        params KeyValuePair<string, Node>[] kvps
    ) : base(kvps) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (StringKeyDictionaryNode)Activator.CreateInstance(typeof(StringKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy StringKeyDictionaryNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static StringKeyDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (StringKeyDictionaryNode)Activator.CreateInstance(typeof(StringKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringKeyDictionaryNode.", ex);
        }
    }

    #endregion

    #region REGION_API

    protected override void OnSet(string key, Node value)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("StringKeyDictionaryNode.SetRemote", new StringNode(key), value);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("StringKeyDictionaryNode.SetRemote", new StringNode(key), value);
    }

    protected override void OnAdd(string key, Node value)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("StringKeyDictionaryNode.AddRemote", new StringNode(key), value);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("StringKeyDictionaryNode.AddRemote", new StringNode(key), value);
    }

    protected override void OnRemove(string key)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("StringKeyDictionaryNode.RemoveRemote", new StringNode(key));
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("StringKeyDictionaryNode.RemoveRemote", new StringNode(key));
    }

    protected override void OnClear()
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("StringKeyDictionaryNode.ClearRemote");
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("StringKeyDictionaryNode.ClearRemote");
    }

    #endregion
}
