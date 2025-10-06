
public class StringKeyDictionaryTailNodeCommon : Node
{

    protected StringKeyDictionaryTailNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll
    ) : base(id_, nodeSyncType_) {}

    public override string ToString()
    {
        return $"{this.GetType().Name}()";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(id);
        writer.Write(nodeSyncType);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
        argsList.Add(reader.ReadInt32());
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
