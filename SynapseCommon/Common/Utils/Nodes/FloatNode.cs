using System;

public class FloatNodeCommon : Node
{
    float f = 0.0f;

    public FloatNodeCommon(float f_ = 0.0f) { f = f_; }

    public override string ToString()
    {
        return $"FloatNode({f})";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeFloat);
        writer.Write(f);
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
        FloatNode node = new FloatNode(1.5f);
        Assert.EqualTrue(NodeStreamer.TestStream(node), "FloatNode changed after serialization and deserialization");
    }
}

#endif
