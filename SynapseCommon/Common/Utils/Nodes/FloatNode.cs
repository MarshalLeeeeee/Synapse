using System;

public class FloatNodeCommon : Node
{
    float f = 0.0f;

    protected FloatNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        float f_ = 0.0f
    ) : base(id_, nodeSyncType_) { f = f_; }

    public override string ToString()
    {
        return $"{this.GetType().Name}({f})";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        argsList.Add(f);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(id);
        writer.Write(nodeSyncType);
        writer.Write(f);
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
        FloatNode node = new FloatNode("", NodeSynConst.SyncAll, -1.5f);
        Assert.EqualTrue(NodeStreamer.TestStream(node), "FloatNode changed after serialization and deserialization");
    }

    public static void TestCopy()
    {
        FloatNode node = new FloatNode("", NodeSynConst.SyncAll, -1.5f);
        FloatNode copy = (FloatNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "FloatNode id not equal after copy");
    }
}

#endif
