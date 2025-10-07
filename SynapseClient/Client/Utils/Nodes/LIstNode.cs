
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

    [Rpc(RpcConst.Server)]
    public void AddRemote(Node child)
    {
        Add(child);
    }

    [Rpc(RpcConst.Server)]
    public void InsertRemote(IntNode index, Node child)
    {
        Insert(index.Get(), child);
    }

    [Rpc(RpcConst.Server)]
    public void RemoveAtRemote(IntNode index)
    {
        RemoveAt(index.Get());
    }

    [Rpc(RpcConst.Server)]
    public void ClearRemote()
    {
        Clear();
    }

    #endregion
}
