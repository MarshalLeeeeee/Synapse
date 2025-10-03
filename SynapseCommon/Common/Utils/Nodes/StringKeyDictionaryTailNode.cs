
public class StringKeyDictionaryTailNodeCommon : Node
{

    public StringKeyDictionaryTailNodeCommon() {}

    public override string ToString()
    {
        return "StringKeyDictionaryTailNode()";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeStringKeyDictionaryTail);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        return argsList.ToArray();
    }

    #endregion
}
