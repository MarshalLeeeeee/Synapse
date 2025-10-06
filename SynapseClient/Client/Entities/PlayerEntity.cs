
[SyncNode(NodeTypeConst.TypePlayerEntity)]
[RegisterEntity]
public class PlayerEntity : PlayerEntityCommon
{
    public override int nodeType => NodeTypeConst.TypePlayerEntity;

    public PlayerEntity(
        string id_ = "",
        Components? components_ = null,
        StringNode? name_ = null, IntNode? money_ = null
    ) : base(id_, components_, name_, money_) { }

    #region REGION_IDENTIFICATION

    public override Node Copy()
    {
        try
        {
            object[] args = GetCopyArgs();
            return (PlayerEntity)Activator.CreateInstance(typeof(PlayerEntity), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to copy PlayerEntity.", ex);
        }
    }

    #endregion

    #region REGION_STREAM

    public static PlayerEntity Deserialize(BinaryReader reader)
    {
        try
        {
            object[] args = DeserializeIntoArgs(reader);
            return (PlayerEntity)Activator.CreateInstance(typeof(PlayerEntity), args);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize PlayerEntity.", ex);
        }
    }

    #endregion
}
