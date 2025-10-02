
[SyncNode(NodeConst.TypeList)]
public class ListNode : ListNodeCommon
{
    public override int nodeType => NodeConst.TypeList;

    public ListNode(params Node[] nodes) : base(nodes) { }

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
}
