
[SyncNode(NodeTypeConst.TypeStringKeyDictionary)]
public class StringKeyDictionaryNode : StringKeyDictionaryNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeStringKeyDictionary;

    public StringKeyDictionaryNode(
        params KeyValuePair<string, Node>[] kvps
    ) : base(kvps) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (StringKeyDictionaryNode)Activator.CreateInstance(typeof(StringKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy StringKeyDictionaryNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static StringKeyDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (StringKeyDictionaryNode)Activator.CreateInstance(typeof(StringKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringKeyDictionaryNode.", ex);
        }
    }

    #endregion
}
