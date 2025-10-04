/// <summary>
/// <para> Basic node of the game tree </para>>
/// <para> Node can be serialized and synchronized from server to client </para>
/// <para> Depending on different client, node can be synchronized differently and partially </para>
/// </summary>

using System.Reflection;

public class NodeTypeConst
{
    public const int TypeUndefined = 0;
    public const int TypeInt = 1;
    public const int TypeFloat = 2;
    public const int TypeString = 3;
    public const int TypeBool = 4;

    public const int TypeList = 10;
    public const int TypeListTail = 11;
    public const int TypeIntKeyDictionary = 12;
    public const int TypeIntKeyDictionaryTail = 13;
    public const int TypeStringKeyDictionary = 14;
    public const int TypeStringKeyDictionaryTail = 15;
}

public class NodeSynConst
{
    public const int SyncNone = 0;
    public const int SyncAll = 1;
    public const int SyncOwn = 2;
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
    public string id = "";

    /// <summary>
    /// the sync type of the node
    /// </summary>
    public int syncType = NodeSynConst.SyncNone;

    /// <summary>
    /// exclusive value for node
    /// </summary>
    public virtual int nodeType => NodeTypeConst.TypeUndefined;

    /// <summary>
    /// Return the string that represents the current node.
    /// </summary>
    public override string ToString()
    {
        return "Node()";
    }

    /// <summary>
    /// Serialize the node into a binary stream.
    /// </summary>
    public virtual void Serialize(BinaryWriter writer) { }
}

public static class NodeStreamer
{
    /// <summary>
    /// Serialize node into byte stream
    /// </summary>
    public static void Serialize(Node node, BinaryWriter writer)
    {
        node.Serialize(writer);
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
        Serialize(node, writer);
        byte[] bd = w_stream.ToArray();

        using var r_stream = new MemoryStream(bd);
        using var reader = new BinaryReader(r_stream);
        Node n = Deserialize(reader);
        return $"{node}" == $"{n}";
    }

#endif
}
