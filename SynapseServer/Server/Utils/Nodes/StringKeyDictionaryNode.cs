
[SyncNode(NodeConst.TypeStringKeyDictionary)]
public class StringKeyDictionaryNode : StringKeyDictionaryNodeCommon
{
    public override int nodeType => NodeConst.TypeStringKeyDictionary;

    public StringKeyDictionaryNode(params KeyValuePair<string, Node>[] kvps) : base(kvps) { }

    public static StringKeyDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (StringKeyDictionaryNode)Activator.CreateInstance(typeof(StringKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringKeyDictionaryNode.", ex);
        }
    }
}

#if DEBUG

[RegisterTest]
public static class TestStringKeyDictionaryNode
{
    public static void TestStream()
    {
        StringKeyDictionaryNode node = new StringKeyDictionaryNode();
        Assert.EqualTrue(NodeStreamer.TestStream(node), "StringKeyDictionaryNode changed after serialization and deserialization");
    }
}

#endif
