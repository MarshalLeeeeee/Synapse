
public class StringKeyDictionaryTailNodeCommon : Node
{

    protected StringKeyDictionaryTailNodeCommon() {}

    public override string ToString()
    {
        return "StringKeyDictionaryTailNode()";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        return argsList.ToArray();
    }

    #endregion
}
