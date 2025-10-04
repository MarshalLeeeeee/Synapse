
[SyncNode(NodeTypeConst.TypeInt)]
public class IntNode : IntNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeInt;

    public IntNode(int v_ = 0) : base(v_) { }

    public static IntNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (IntNode)Activator.CreateInstance(typeof(IntNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntNode.", ex);
        }
    }
}
