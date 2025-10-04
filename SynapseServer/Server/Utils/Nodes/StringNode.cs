
[SyncNode(NodeTypeConst.TypeString)]
public class StringNode : StringNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeString;

    public StringNode(string s_ = "") : base(s_) { }

    public static StringNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (StringNode)Activator.CreateInstance(typeof(StringNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringNode.", ex);
        }
    }
}
