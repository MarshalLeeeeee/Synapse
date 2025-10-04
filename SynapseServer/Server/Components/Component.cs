
public class Component : ComponentCommon
{

}

[SyncNode(NodeTypeConst.TypeComponents)]
public class Components : ComponentsCommon
{
    public override int nodeType => NodeTypeConst.TypeComponents;

    public Components(params KeyValuePair<string, Component>[] kvps) : base(kvps) { }

    public static Components Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (Components)Activator.CreateInstance(typeof(Components), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntKeyDictionaryNode.", ex);
        }
    }
}
