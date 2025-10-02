
public class ListTailNodeCommon : Node
{

    public ListTailNodeCommon() { }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeListTail);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        return [];
    }

    #endregion
}
