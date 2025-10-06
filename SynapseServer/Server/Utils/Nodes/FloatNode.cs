
[SyncNode(NodeTypeConst.TypeFloat)]
public class FloatNode : FloatNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeFloat;

    public FloatNode(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        float f_ = 0.0f
    ) : base(id_, nodeSyncType_, f_) { }

    public static FloatNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (FloatNode)Activator.CreateInstance(typeof(FloatNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize FloatNode.", ex);
        }
    }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (FloatNode)Activator.CreateInstance(typeof(FloatNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy FloatNode.", ex);
        }
    }

    #endregion
}
