
public class BoolNodeCommon : Node
{
    bool v = false;

    protected BoolNodeCommon(
        string id_ = "",
        bool v_ = false
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
            writer.Write(false);
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
        BoolNode node = new BoolNode("", true);
        Assert.EqualTrue(NodeStreamer.TestStream(node), "BoolNode changed after serialization and deserialization");
    }

    public static void TestCopy()
    {
        BoolNode node = new BoolNode("", true);
        BoolNode copy = (BoolNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "BoolNode id not equal after copy");
    }
}

#endif

