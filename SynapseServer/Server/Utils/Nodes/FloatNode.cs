
[SyncNode(NodeTypeConst.TypeFloat)]
public class FloatNode : FloatNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeFloat;

    public FloatNode(float f_ = 0.0f) : base(f_) { }

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
}
