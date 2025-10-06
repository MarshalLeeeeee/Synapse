
public class Entity : EntityCommon
{
    protected Entity(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        Components? components_ = null
    ) : base(id_, nodeSyncType_, components_) { }

    public static Entity Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (Entity)Activator.CreateInstance(typeof(Entity), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize Entity.", ex);
        }
    }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (Entity)Activator.CreateInstance(typeof(Entity), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy Entity.", ex);
        }
    }

    #endregion
}
