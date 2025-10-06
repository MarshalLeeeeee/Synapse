
[SyncNode(NodeTypeConst.TypeStringKeyDictionaryTail)]
public class StringKeyDictionaryTailNode : StringKeyDictionaryTailNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeStringKeyDictionaryTail;

    public StringKeyDictionaryTailNode(string id_ = "") : base(id_) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (StringKeyDictionaryTailNode)Activator.CreateInstance(typeof(StringKeyDictionaryTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy StringKeyDictionaryTailNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static StringKeyDictionaryTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (StringKeyDictionaryTailNode)Activator.CreateInstance(typeof(StringKeyDictionaryTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringKeyDictionaryTailNode.", ex);
        }
    }

    #endregion
}
