
public class StringNodeCommon : Node
{
    protected string s = "";

    protected StringNodeCommon(
        string s_ = ""
    ) : base() { s = s_; }

    public override string ToString()
    {
        return $"{this.GetType().Name}({s})";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(s);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        if (ShouldSerializeContent(proxyId))
        {
            writer.Write(s);
        }
        else
        {
            writer.Write("");
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
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public string Get()
    {
        return s;
    }

    public void Set(string s_)
    {
        if (s == s_) return;
        s = s_;
        OnSet();
    }

    protected virtual void OnSet() {}

    #endregion
}

#if DEBUG

[RegisterTest]
public static class TestStringNode
{
    public static void TestStream()
    {
        StringNode node = new StringNode("Test");
        Assert.EqualTrue(NodeStreamer.TestStream(node), "StringNode changed after serialization and deserialization");
    }

    public static void TestCopy()
    {
        StringNode node = new StringNode("Test");
        StringNode copy = (StringNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "StringNode id not equal after copy");
    }
}

#endif
