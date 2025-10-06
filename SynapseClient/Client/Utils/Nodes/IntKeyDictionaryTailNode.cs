
[SyncNode(NodeTypeConst.TypeIntKeyDictionaryTail)]
public class IntKeyDictionaryTailNode : IntKeyDictionaryTailNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeIntKeyDictionaryTail;

    public IntKeyDictionaryTailNode(string id_ = "") : base(id_) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (IntKeyDictionaryTailNode)Activator.CreateInstance(typeof(IntKeyDictionaryTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy IntKeyDictionaryTailNode.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static IntKeyDictionaryTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (IntKeyDictionaryTailNode)Activator.CreateInstance(typeof(IntKeyDictionaryTailNode), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize IntKeyDictionaryTailNode.", ex);
        }
    }

    #endregion
}
