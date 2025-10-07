
[SyncNode(NodeTypeConst.TypeBool)]
public class BoolNode : BoolNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeBool;

    public BoolNode(
        bool v_ = false
    ) : base(v_) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (BoolNode)Activator.CreateInstance(typeof(BoolNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy BoolNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static BoolNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (BoolNode)Activator.CreateInstance(typeof(BoolNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize BoolNode.", ex);
        }
    }

    #endregion

    #region REGION_API

    protected override void OnSet()
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("BoolNode.SetRemote", this);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("BoolNode.SetRemote", this);
    }

    #endregion
}
