
[SyncNode(NodeTypeConst.TypeString)]
public class StringNode : StringNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeString;

    public StringNode(
        string s_ = ""
    ) : base(s_) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (StringNode)Activator.CreateInstance(typeof(StringNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy StringNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static StringNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (StringNode)Activator.CreateInstance(typeof(StringNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringNode.", ex);
        }
    }

    #endregion

    #region REGION_API

    protected override void OnSet()
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("StringNode.SetRemote", this);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("StringNode.SetRemote", this);
    }

    #endregion
}
