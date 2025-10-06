
[SyncNode(NodeTypeConst.TypeBool)]
public class BoolNode : BoolNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeBool;

    public BoolNode(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        bool v_ = false
    ) : base(id_, nodeSyncType_, v_) { }

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
}
