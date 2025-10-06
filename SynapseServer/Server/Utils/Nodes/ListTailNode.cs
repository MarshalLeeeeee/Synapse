
[SyncNode(NodeTypeConst.TypeListTail)]
public class ListTailNode : ListTailNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeListTail;

    public ListTailNode(string id_ = "") : base(id_) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (ListTailNode)Activator.CreateInstance(typeof(ListTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy ListTailNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static ListTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (ListTailNode)Activator.CreateInstance(typeof(ListTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize ListTailNode.", ex);
        }
    }

    #endregion
}
