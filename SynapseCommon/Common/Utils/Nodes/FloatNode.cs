using System;

public class FloatNodeCommon : Node
{
    float f = 0.0f;

    protected FloatNodeCommon(
        float f_ = 0.0f
    ) : base() { f = f_; }

    public override string ToString()
    {
        return $"{this.GetType().Name}({f})";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(f);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        if (ShouldSerializeContent(proxyId))
        {
            writer.Write(f);
        }
        else
        {
            writer.Write(0.0f);
        }
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadSingle());
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public float Get()
    {
        return f;
    }

    public void Set(float f_)
    {
        if (Math.Abs(f - f_) < 1e-3) return;
        f = f_;
    }

    #endregion
}

#if DEBUG

[RegisterTest]
public static class TestFloatNode
{
    public static void TestStream()
    {
        FloatNode node = new FloatNode(-1.5f);
        Assert.EqualTrue(NodeStreamer.TestStream(node), "FloatNode changed after serialization and deserialization");
    }

    public static void TestCopy()
    {
        FloatNode node = new FloatNode(-1.5f);
        FloatNode copy = (FloatNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "FloatNode id not equal after copy");
    }
}

#endif
