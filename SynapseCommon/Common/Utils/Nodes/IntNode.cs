
public class IntNodeCommon : Node
{
    int v = 0;

    public IntNodeCommon(int v_ = 0) { v = v_; }

    public override string ToString()
    {
        return $"IntNode({v})";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeInt);
        writer.Write(v);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
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
        IntNode node = new IntNode(3);
        Assert.EqualTrue(NodeStreamer.TestStream(node), "IntNode changed after serialization and deserialization");
    }
}

#endif
