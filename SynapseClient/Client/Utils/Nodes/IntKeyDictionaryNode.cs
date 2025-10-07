
[SyncNode(NodeTypeConst.TypeIntKeyDictionary)]
public class IntKeyDictionaryNode : IntKeyDictionaryNodeCommon
{
    public override int nodeType => NodeTypeConst.TypeIntKeyDictionary;

    public IntKeyDictionaryNode(
        params KeyValuePair<int, Node>[] kvps
    ) : base(kvps) { }

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

    #region REGION_API

    [Rpc(RpcConst.Server)]
    public void SetRemote(IntNode key, Node value)
    {
        this[key.Get()] = value;
    }

    [Rpc(RpcConst.Server)]
    public void AddRemote(IntNode key, Node value)
    {
        Add(key.Get(), value);
    }

    [Rpc(RpcConst.Server)]
    public void RemoveRemote(IntNode key)
    {
        Remove(key.Get());
    }

    [Rpc(RpcConst.Server)]
    public void ClearRemote()
    {
        Clear();
    }

    #endregion
}

