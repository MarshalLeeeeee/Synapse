
using System.Xml.Linq;

public class PlayerEntityCommon : Entity
{
    protected StringNode name = new StringNode();
    protected IntNode money = new IntNode();

    protected PlayerEntityCommon(
        string id_ = "",
        Components? components_ = null,
        StringNode? name_ = null, IntNode? money_ = null
    ) : base(id_, components_)
    {
        if (name_ != null) name = name_;
        if (money_ != null) money = money_;
    }

    public override string ToString()
    {
        return $"{this.GetType().Name}(id: {id}, name: {name}, money: {money}, )";
    }

    #region REGION_IDENTIFICATION

    public override void SetId(string id_)
    {
        base.SetId(id_);
        name.SetId(id_ + ".name");
        money.SetId(id_ + ".money");
    }

    public override Node? GetChildWithId(string id_)
    {
        if (id_ == "name") return name;
        if (id_ == "money") return money;
        return base.GetChildWithId(id_);
    }

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add("");
        argsList.Add(components);
        argsList.Add(name);
        argsList.Add(money);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        writer.Write(id);
        NodeStreamer.Serialize(components, writer, proxyId);
        NodeStreamer.Serialize(name, writer, proxyId);
        NodeStreamer.Serialize(money, writer, proxyId);
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
