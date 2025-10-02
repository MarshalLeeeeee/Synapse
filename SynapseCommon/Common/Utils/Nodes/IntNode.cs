
public class IntNodeCommon : Node
{
    /* data */
    int v = 0;

    public IntNodeCommon(int v_ = 0) { v = v_; }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeInt);
        writer.Write(v);
    }

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
