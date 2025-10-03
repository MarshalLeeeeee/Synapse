
[SyncNode(NodeConst.TypeStringKeyDictionaryTail)]
public class StringKeyDictionaryTailNode : StringKeyDictionaryTailNodeCommon
{
    public override int nodeType => NodeConst.TypeStringKeyDictionaryTail;

    public StringKeyDictionaryTailNode() : base() { }

    public static StringKeyDictionaryTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (StringKeyDictionaryTailNode)Activator.CreateInstance(typeof(StringKeyDictionaryTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringKeyDictionaryTailNode.", ex);
        }
    }
}

#if DEBUG

[RegisterTest]
public static class TestStringKeyDictionaryTailNode
{
    public static void TestStream()
    {
        StringKeyDictionaryTailNode node = new StringKeyDictionaryTailNode();
        Assert.EqualTrue(NodeStreamer.TestStream(node), "StringKeyDictionaryTailNode changed after serialization and deserialization");
    }
}

#endif
