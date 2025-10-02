
public class BoolNodeCommon : Node
{
    /* data */
    bool v = false;

    public BoolNodeCommon(bool v_ = false) { v = v_; }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeBool);
        writer.Write(v);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadBoolean);
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
