
using System.Xml.Linq;

public class PlayerEntityCommon : Entity
{
    protected string account = "";
    protected StringNode name = new StringNode();
    protected IntNode money = new IntNode();

    protected PlayerEntityCommon(
        Components? components_ = null,
        string account_ = "",
        StringNode? name_ = null, IntNode? money_ = null
    ) : base(components_)
    {
        account = account_;
        if (name_ != null) name = name_;
        if (money_ != null) money = money_;
    }

    public override string ToString()
    {
        return $"{this.GetType().Name}(account: {account}, name: {name}, money: {money}, )";
    }

    #region REGION_IDENTIFICATION

    public override void SetId(string id_, Node? parent_ = null)
    {
        base.SetId(id_, parent_);
        name.SetId("name", this);
        money.SetId("money", this);
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
        argsList.Add(components.Copy());
        argsList.Add(account);
        argsList.Add(name.Copy());
        argsList.Add(money.Copy());
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        NodeStreamer.Serialize(components, writer, proxyId);
        writer.Write(account);
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
        argsList.Add(NodeStreamer.Deserialize(reader));
        argsList.Add(reader.ReadString());
        argsList.Add(NodeStreamer.Deserialize(reader));
        argsList.Add(NodeStreamer.Deserialize(reader));
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public string GetAccount()
    {
        return account;
    }

    public string GetName()
    {
        return name.Get();
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
