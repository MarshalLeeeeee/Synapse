/// <summary>
/// <para> Basic node of the game tree </para>>
/// <para> Node can be serialized and synchronized from server to client </para>
/// <para> Depending on different client, node can be synchronized differently and partially </para>
/// </summary>

using System.Reflection;

public class NodeTypeConst
{
    #region REGION_BASE

    public const int TypeUndefined = 0;
    public const int TypeInt = 1;
    public const int TypeFloat = 2;
    public const int TypeString = 3;
    public const int TypeBool = 4;

    public const int TypeList = 11;
    public const int TypeListTail = 12;
    public const int TypeIntKeyDictionary = 13;
    public const int TypeIntKeyDictionaryTail = 14;
    public const int TypeStringKeyDictionary = 15;
    public const int TypeStringKeyDictionaryTail = 16;

    #endregion

    #region REGION_ENTITY

    public const int EntityOffset = 1000000;
    public const int TypePlayerEntity = EntityOffset + 1;

    #endregion

    #region REGION_COMPONENT

    public const int ComponentOffset = 2000000;
    public const int TypeComponents = ComponentOffset;

    #endregion
}

public class NodeSyncConst
{
    public const int SyncAll = 0;
    public const int SyncOwn = 1;
}

/// <summary>
/// Use it when a node needs to be synced from server to client
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SyncNodeAttribute : Attribute
{
    public int nodeType { get; }

    public SyncNodeAttribute(int nodeType_)
    {
        nodeType = nodeType_;
    }
}

public class NodeCommon
{
    /// <summary>
    /// Every node has a unique id at runtime which serves as the identifier of the node in both server and client. 
    /// </summary>
    public string id { get; protected set; } = "";

    /// <summary>
    /// Parent node of the current node, null if the node is root
    /// </summary>
    protected Node? parent = null;

    /// <summary>
    /// exclusive value for node
    /// </summary>
    public virtual int nodeType => NodeTypeConst.TypeUndefined;

    protected NodeCommon() {}

    /// <summary>
    /// Return the string that represents the current node.
    /// </summary>
    public override string ToString()
    {
        return $"{this.GetType().Name}()";
    }

    #region REGION_IDENTIFICATION

    /// <summary>
    /// set id and parent of the node
    /// </summary>
    /// <param name="id_"> id of this node </param>
    /// <param name="parent_"> parent node, null if no parent node </param>
    public virtual void SetId(string id_, Node? parent_ = null)
    {
        id = id_;
        parent = parent_;
    }

    /// <summary>
    /// get id of the root node
    /// </summary>
    /// <returns> id of the root node </returns>
    public string GetRootId()
    {
        if (parent == null) return id;
        return parent.GetRootId();
    }

    /// <summary>
    /// get full id of the node along the path from root to current node
    /// </summary>
    /// <returns></returns>
    public string GetFullId()
    {
        if (parent == null) return id;
        return $"{parent.GetFullId()}.{id}";
    }

    /// <summary>
    /// get child node with specific id
    /// </summary>
    /// <param name="id_"> string: node id </param>
    /// <returns> Node instance if target child exists, null otherwise </returns>
    public virtual Node? GetChildWithId(string id_)
    {
        return null;
    }

    /// <summary>
    /// Get arguments for constructor to create a copy of the node.
    /// </summary>
    /// <returns> arguments for constructor </returns>
    public virtual object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add("");
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    /// <summary>
    /// if the node should be synchronized to the client with proxyId
    /// </summary>
    /// <param name="proxyId"> proxy id </param>
    /// <returns> True if the node should be synchronized to the client with proxyId </returns>
    public virtual bool ShouldSerializeContent(string proxyId)
    {
        return true;
    }

    /// <summary>
    /// Serialize the node into a binary stream.
    /// </summary>
    public virtual void Serialize(BinaryWriter writer, string proxyId) { }

    #endregion
}

public static class NodeStreamer
{
    /// <summary>
    /// Serialize node into byte stream
    /// </summary>
    public static void Serialize(Node node, BinaryWriter writer, string proxyId)
    {
        node.Serialize(writer, proxyId);
    }

    /// <summary>
    /// Deserialize bytes from reader into specific node
    /// </summary>
    /// <exception cref="InvalidDataException"> throw when deserialization fails </exception>
    public static Node Deserialize(BinaryReader reader)
    {
        try
        {
            int type = reader.ReadInt32();
            MethodInfo? deserializeMethod = Reflection.GetDeserializeMethod(type);
            if (deserializeMethod != null)
            {
                object? v = deserializeMethod.Invoke(null, new object[] { reader });
                if (v != null && v is Node node) return node;
                else
                {
                    throw new InvalidDataException($"Deserialization does not return Type Node: {type}");
                }
            }
            else
            {
                throw new InvalidDataException($"Unsupported node type: {type}");
            }
        }
        catch
        {
            throw new InvalidDataException("Failed to deserialize Node.");
        }
    }

#if DEBUG

    /// <summary>
    /// Check if serialize and deserialize return the identical node
    /// </summary>
    /// <param name="node"> the node object </param>
    public static bool TestStream(Node node)
    {
        using var w_stream = new MemoryStream();
        using var writer = new BinaryWriter(w_stream);
        Serialize(node, writer, "");
        byte[] bd = w_stream.ToArray();

        using var r_stream = new MemoryStream(bd);
        using var reader = new BinaryReader(r_stream);
        Node n = Deserialize(reader);
        return $"{node}" == $"{n}";
    }

#endif
}
