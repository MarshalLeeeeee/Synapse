
public class IntKeyDictionaryTailNodeCommon : Node
{

    public IntKeyDictionaryTailNodeCommon() {}

    public override string ToString()
    {
        return "IntKeyDictionaryTailNode()";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeIntKeyDictionaryTail);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        return argsList.ToArray();
    }

    #endregion
}
