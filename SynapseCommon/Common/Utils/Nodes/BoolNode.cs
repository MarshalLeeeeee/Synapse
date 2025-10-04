
public class BoolNodeCommon : Node
{
    bool v = false;

    protected BoolNodeCommon(bool v_ = false) { v = v_; }

    public override string ToString()
    {
        return $"BoolNode({v})";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(v);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadBoolean());
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public bool Get()
    {
        return v;
    }

    public void Set(bool v_)
    {
        if (v == v_) return;
        v = v_;
    }

    #endregion
}

#if DEBUG

[RegisterTest]
public static class TestBoolNode
{
    public static void TestStream()
    {
        BoolNode node = new BoolNode(true);
        Assert.EqualTrue(NodeStreamer.TestStream(node), "BoolNode changed after serialization and deserialization");
    }
}

#endif

