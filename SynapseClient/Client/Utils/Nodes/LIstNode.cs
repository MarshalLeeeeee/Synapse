
[SyncNode(NodeTypeConst.TypeList)]
public class ListNode : ListNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeList;

    public ListNode(
        string id_ = "",
        params Node[] nodes
    ) : base(id_, nodes) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (ListNode)Activator.CreateInstance(typeof(ListNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy ListNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static ListNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (ListNode)Activator.CreateInstance(typeof(ListNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize ListNode.", ex);
        }
    }

    #endregion
}
