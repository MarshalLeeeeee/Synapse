
public class StringNodeCommon : Node
{
    protected string s = "";

    public StringNodeCommon(string s_ = "") { s = s_; }

    public override string ToString()
    {
        return $"StringNode({s})";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeString);
        writer.Write(s);
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
    }

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
}

#endif
