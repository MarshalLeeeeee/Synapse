
[SyncNode(NodeTypeConst.TypeBool)]
public class BoolNode : BoolNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeBool;

    public BoolNode(bool v_ = false) : base(v_) { }

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
}
