
public class StringKeyDictionaryTailNodeCommon : Node
{

    protected StringKeyDictionaryTailNodeCommon(string id_ = "") : base(id_) {}

    public override string ToString()
    {
        return $"{this.GetType().Name}()";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add("");
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        writer.Write(id);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
        return argsList.ToArray();
    }

    #endregion
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

    public static void TestCopy()
    {
        StringKeyDictionaryTailNode node = new StringKeyDictionaryTailNode();
        StringKeyDictionaryTailNode copy = (StringKeyDictionaryTailNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "StringKeyDictionaryTailNode id not equal after copy");
    }
}

#endif
