
using System.Xml.Linq;

public class PlayerEntityCommon : Entity
{
    protected StringNode name = new StringNode("Name");
    protected IntNode money = new IntNode(999);

    protected PlayerEntityCommon(
        string eid = "",
        StringNode? name_ = null,
        IntNode? money_ = null
    ) : base(eid)
    {
        if (name_ != null) name = name_;
        if (money_ != null) money = money_;
    }

    public override string ToString()
    {
        return $"PlayerEntity(name: {name}, money: {money}, )";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(entityId);
        NodeStreamer.Serialize(name, writer);
        NodeStreamer.Serialize(money, writer);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
        argsList.Add(NodeStreamer.Deserialize(reader));
        argsList.Add(NodeStreamer.Deserialize(reader));
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public void SetName(string name_)
    {
        name.Set(name_);
    }

    public string GetName()
    {
        return name.Get();
    }

    public void SetMoney(int money_)
    {
        money.Set(money_);
    }

    public int GetMoney()
    {
        return money.Get();
    }

    #endregion
}

#if DEBUG

[RegisterGm]
public static class GmShowPlayer
{
    public static void Execute(string playerId)
    {
        PlayerEntity? player = Game.Instance.GetManager<EntityManager>()?.GetPlayerEntity(playerId);
        if (player != null)
        {
            Log.Debug($"{player}");
        }
    }
}

#endif

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
