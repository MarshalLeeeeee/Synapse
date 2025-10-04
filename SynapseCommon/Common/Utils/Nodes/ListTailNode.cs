
public class ListTailNodeCommon : Node
{

    public ListTailNodeCommon() { }

    public override string ToString()
    {
        return "ListTailNode()";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeTypeConst.TypeListTail);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        return [];
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
}

#endif
