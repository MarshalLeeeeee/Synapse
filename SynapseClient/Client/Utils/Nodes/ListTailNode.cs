
[SyncNode(NodeConst.TypeListTail)]
public class ListTailNode : ListTailNodeCommon
{
    /* dynamic data type */
    public override int nodeType => NodeConst.TypeListTail;

    public ListTailNode() : base() { }

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
}
