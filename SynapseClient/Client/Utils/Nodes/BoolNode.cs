
[SyncNode(NodeTypeConst.TypeBool)]
public class BoolNode : BoolNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeBool;

    public BoolNode(
        string id_ = "",
        bool v_ = false
    ) : base(id_, v_) { }

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
}
