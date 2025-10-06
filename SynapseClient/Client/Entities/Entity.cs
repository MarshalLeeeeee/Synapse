
public class Entity : EntityCommon
{
    protected Entity(
        string id_ = "",
        Components? components_ = null
    ) : base(id_, components_) { }

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

    #region REGION_STREAM

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

    #endregion
}
