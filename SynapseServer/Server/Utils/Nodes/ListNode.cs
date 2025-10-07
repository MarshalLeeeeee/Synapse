
[SyncNode(NodeTypeConst.TypeList)]
public class ListNode : ListNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeList;

    public ListNode(
        params Node[] nodes
    ) : base(nodes) { }

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

    #region REGION_API

    protected override void OnAdd(Node child)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("ListNode.AddRemote", child);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("ListNode.AddRemote", child);
    }

    protected override void OnInsert(int index, Node child)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("ListNode.InsertRemote", new IntNode(index), child);
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("ListNode.InsertRemote", new IntNode(index), child);
    }

    protected override void OnRemoveAt(int index)
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("ListNode.RemoveAtRemote",  new IntNode(index));
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("ListNode.RemoveAtRemote",  new IntNode(index));
    }

    protected override void OnClear()
    {
        if (nodeSyncType == NodeSyncConst.SyncAll) SyncAll("ListNode.ClearRemote");
        else if (nodeSyncType == NodeSyncConst.SyncOwn) SyncOwn("ListNode.ClearRemote");
    }

    #endregion
}
