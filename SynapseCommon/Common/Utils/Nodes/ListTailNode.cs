
public class ListTailNodeCommon : Node
{

    protected ListTailNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll
    ) : base(id_, nodeSyncType_) { }

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

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
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
public static class TestListTailNode
{
    public static void TestStream()
    {
        ListTailNode node = new ListTailNode();
        Assert.EqualTrue(NodeStreamer.TestStream(node), "ListTailNode changed after serialization and deserialization");
    }

    public static void TestCopy()
    {
        ListTailNode node = new ListTailNode();
        ListTailNode copy = (ListTailNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "ListTailNode id not equal after copy");
    }
}

#endif
