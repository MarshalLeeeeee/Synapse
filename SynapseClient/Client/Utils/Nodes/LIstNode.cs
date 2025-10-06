
[SyncNode(NodeTypeConst.TypeList)]
public class ListNode : ListNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeList;

    public ListNode(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        params Node[] nodes
    ) : base(id_, nodeSyncType_, nodes) { }

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
}
