
public class StringNodeCommon : Node
{
    /* data */
    protected string s = "";

    public StringNodeCommon(string s_ = "") { s = s_; }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeString);
        writer.Write(s);
    }

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