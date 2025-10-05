
[SyncNode(NodeTypeConst.TypePlayerEntity)]
[RegisterEntity]
public class PlayerEntity : PlayerEntityCommon
{
    public override int nodeType => NodeTypeConst.TypePlayerEntity;

    public PlayerEntity(
        string eid = "",
        StringNode? name_ = null,
        IntNode? money_ = null
    ) : base(eid, name_, money_) { }

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

    protected override void InitComponents()
    {
        
    }
}
