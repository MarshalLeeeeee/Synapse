
[SyncNode(NodeTypeConst.TypePlayerEntity)]
[RegisterEntity]
public class PlayerEntity : PlayerEntityCommon
{
    public override int nodeType => NodeTypeConst.TypePlayerEntity;

    public PlayerEntity(
        Components? components_ = null,
        string account_ = "",
        StringNode? name_ = null, IntNode? money_ = null
    ) : base(components_, account_, name_, money_)
    {
        name.SetNodeSyncType(NodeSyncConst.SyncAll);
        name.Set("Name");
        money.SetNodeSyncType(NodeSyncConst.SyncOwn);
        money.Set(999);
    }

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

    #region REGION_API

    public void SetAccount(string account_)
    {
        account = account_;
    }

    public void SetName(string name_)
    {
        name.Set(name_);
    }

    public void SetMoney(int money_)
    {
        money.Set(money_);
    }

    #endregion
}

#if DEBUG

[RegisterGm]
public static class GmSetPlayerName
{
    public static void Execute(string playerId, string name)
    {
        PlayerEntity? player = Game.Instance.GetManager<EntityManager>()?.GetPlayerEntity(playerId);
        if (player != null)
        {
            player.SetName(name);
        }
    }
}

#endif

#if DEBUG

[RegisterGm]
public static class GmSetPlayerMoney
{
    public static void Execute(string playerId, int money)
    {
        PlayerEntity? player = Game.Instance.GetManager<EntityManager>()?.GetPlayerEntity(playerId);
        if (player != null)
        {
            player.SetMoney(money);
        }
    }
}

#endif
