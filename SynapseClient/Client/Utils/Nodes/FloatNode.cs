
[SyncNode(NodeTypeConst.TypeFloat)]
public class FloatNode : FloatNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeFloat;

    public FloatNode(
        float f_ = 0.0f
    ) : base(f_) { }

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

    #region REGION_STREAM

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

    #endregion

    #region REGION_API

    [Rpc(RpcConst.Server, NodeTypeConst.TypeFloat)]
    public void SetRemote(FloatNode node)
    {
        Set(node.Get());
    }

    #endregion
}
