
[SyncNode(NodeTypeConst.TypeInt)]
public class IntNode : IntNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeInt;

    public IntNode(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        int v_ = 0
    ) : base(id_, nodeSyncType_, v_) { }

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
}
