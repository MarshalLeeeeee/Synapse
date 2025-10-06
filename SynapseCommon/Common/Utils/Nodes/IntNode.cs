
public class IntNodeCommon : Node
{
    int v = 0;

    protected IntNodeCommon(
        string id_ = "",
        int v_ = 0
    ) : base(id_) { v = v_; }

    public override string ToString()
    {
        return $"{this.GetType().Name}({v})";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add("");
        argsList.Add(v);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        writer.Write(id);
        if (ShouldSerializeContent(proxyId))
        {
            writer.Write(v);
        }
        else
        {
            writer.Write(0);
        }
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

    #region REGION_API

    public int Get()
    {
        return v;
    }

    public void Set(int v_)
    {
        if (v == v_) return;
        v = v_;
    }

    #endregion
}

#if DEBUG

[RegisterTest]
public static class TestIntNode
{
    public static void TestStream()
    {
        IntNode node = new IntNode("", 3);
        Assert.EqualTrue(NodeStreamer.TestStream(node), "IntNode changed after serialization and deserialization");
    }

    public static void TestCopy()
    {
        IntNode node = new IntNode("", 3);
        IntNode copy = (IntNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "IntNode id not equal after copy");
    }
}

#endif
