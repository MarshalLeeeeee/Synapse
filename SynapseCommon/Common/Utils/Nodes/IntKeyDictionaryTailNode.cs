
public class IntKeyDictionaryTailNodeCommon : Node
{

    protected IntKeyDictionaryTailNodeCommon() : base() {}

    public override string ToString()
    {
        return $"{this.GetType().Name}()";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        return argsList.ToArray();
    }

    #endregion
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

    public static void TestCopy()
    {
        IntKeyDictionaryTailNode node = new IntKeyDictionaryTailNode();
        IntKeyDictionaryTailNode copy = (IntKeyDictionaryTailNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "IntKeyDictionaryTailNode id not equal after copy");
    }
}

#endif
