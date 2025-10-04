
[SyncNode(NodeTypeConst.TypeIntKeyDictionaryTail)]
public class IntKeyDictionaryTailNode : IntKeyDictionaryTailNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeIntKeyDictionaryTail;

    public IntKeyDictionaryTailNode() : base() { }

    public static IntKeyDictionaryTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (IntKeyDictionaryTailNode)Activator.CreateInstance(typeof(IntKeyDictionaryTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntKeyDictionaryTailNode.", ex);
        }
    }
}

#if DEBUG

[RegisterTest]
public static class TestIntKeyDictionaryTailNode
{
    public static void TestStream()
    {
        IntKeyDictionaryTailNode node = new IntKeyDictionaryTailNode();
        Assert.EqualTrue(NodeStreamer.TestStream(node), "IntKeyDictionaryTailNode changed after serialization and deserialization");
    }
}

#endif
