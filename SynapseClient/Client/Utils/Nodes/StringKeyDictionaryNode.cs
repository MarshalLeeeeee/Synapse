
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
        StringKeyDictionaryNode node = new StringKeyDictionaryNode(
            new KeyValuePair<string, Node>("0", new IntNode(3)),
            new KeyValuePair<string, Node>("1", new FloatNode(3.3f)),
            new KeyValuePair<string, Node>("2", new StringNode("3")),
            new KeyValuePair<string, Node>("3", new ListNode(new IntNode(4), new FloatNode(5.0f))),
            new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode(
                new KeyValuePair<string, Node>("0", new IntNode(3)),
                new KeyValuePair<string, Node>("1", new FloatNode(3.3f)),
                new KeyValuePair<string, Node>("2", new StringNode("3")),
                new KeyValuePair<string, Node>("3", new ListNode(new IntNode(4), new FloatNode(5.0f))),
                new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode())
            ))
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "StringKeyDictionaryNode changed after serialization and deserialization");
    }
}

#endif
