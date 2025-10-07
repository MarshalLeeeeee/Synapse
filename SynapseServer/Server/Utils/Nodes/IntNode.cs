
[SyncNode(NodeTypeConst.TypeInt)]
public class IntNode : IntNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeInt;

    public IntNode(
        int v_ = 0
    ) : base(v_) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (IntNode)Activator.CreateInstance(typeof(IntNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy IntNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static IntNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (IntNode)Activator.CreateInstance(typeof(IntNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntNode.", ex);
        }
    }

    #endregion

    #region REGION_API

    protected override void OnSet()
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("IntNode.SetRemote", this);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("IntNode.SetRemote", this);
    }

    #endregion
}
