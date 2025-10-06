
using System.Xml.Linq;

public class PlayerEntityCommon : Entity
{
    protected StringNode name = new StringNode("name", NodeSynConst.SyncAll, "Name");
    protected IntNode money = new IntNode("money", NodeSynConst.SyncOwn, 999);

    protected PlayerEntityCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        Components? components_ = null,
        StringNode? name_ = null, IntNode? money_ = null
    ) : base(id_, nodeSyncType_, components_)
    {
        if (name_ != null) name = name_;
        if (money_ != null) money = money_;
    }

    public override string ToString()
    {
        return $"{this.GetType().Name}(id: {id}, name: {name}, money: {money}, )";
    }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        argsList.Add(components);
        argsList.Add(name);
        argsList.Add(money);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(id);
        writer.Write(nodeSyncType);
        NodeStreamer.Serialize(components, writer);
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
        argsList.Add(reader.ReadInt32());
        argsList.Add(NodeStreamer.Deserialize(reader));
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
