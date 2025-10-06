
[SyncNode(NodeTypeConst.TypeIntKeyDictionary)]
public class IntKeyDictionaryNode : IntKeyDictionaryNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeIntKeyDictionary;

    public IntKeyDictionaryNode(
        string id_ = "",
        params KeyValuePair<int, Node>[] kvps
    ) : base(id_, kvps) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (IntKeyDictionaryNode)Activator.CreateInstance(typeof(IntKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy IntKeyDictionaryNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static IntKeyDictionaryNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (IntKeyDictionaryNode)Activator.CreateInstance(typeof(IntKeyDictionaryNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntKeyDictionaryNode.", ex);
        }
    }

    #endregion
}

