
[SyncNode(NodeTypeConst.TypeIntKeyDictionary)]
public class IntKeyDictionaryNode : IntKeyDictionaryNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeIntKeyDictionary;

    public IntKeyDictionaryNode(params KeyValuePair<int, Node>[] kvps) : base(kvps) { }

    public static IntKeyDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (IntKeyDictionaryNode)Activator.CreateInstance(typeof(IntKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntKeyDictionaryNode.", ex);
        }
    }
}

#if DEBUG

[RegisterTest]
public static class TestIntKeyDictionaryNode
{
    public static void TestStream()
    {
        IntKeyDictionaryNode node = new IntKeyDictionaryNode(
            new KeyValuePair<int, Node>(0, new IntNode(3)),
            new KeyValuePair<int, Node>(1, new FloatNode(3.3f)),
            new KeyValuePair<int, Node>(2, new StringNode("3")),
            new KeyValuePair<int, Node>(3, new ListNode(new IntNode(4), new FloatNode(5.0f))),
            new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode(
                new KeyValuePair<int, Node>(0, new IntNode(3)),
                new KeyValuePair<int, Node>(1, new FloatNode(3.3f)),
                new KeyValuePair<int, Node>(2, new StringNode("3")),
                new KeyValuePair<int, Node>(3, new ListNode(new IntNode(4), new FloatNode(5.0f))),
                new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode())
            ))
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "IntKeyDictionaryNode changed after serialization and deserialization");
    }
}

#endif
