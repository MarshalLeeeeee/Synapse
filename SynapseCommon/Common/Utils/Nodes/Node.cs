/*
 * Basic node of the game tree
 * Node can be serialized and synchronized from server to client
 * Depending on different client, node can be synchronized differently and partially
 */

using System.Reflection;

public class NodeConst
{
    public const int TypeUndefined = 0;
    public const int TypeInt = 1;
    public const int TypeFloat = 2;
    public const int TypeString = 3;
    public const int TypeBool = 4;

    public const int TypeList = 10;
    public const int TypeListTail = 11;
}

/* Use it when a node needs to be synced from server to client */
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SyncNodeAttribute : Attribute
{
    public int nodeType { get; }

    public SyncNodeAttribute(int nodeType_)
    {
        nodeType = nodeType_;
    }
}

public class Node
{
    /* Every node has a unique id at runtime，
     * which serves as the identifier of the node in both server and client. 
     */
    public string id = "";
    /* dynamic data type */
    public virtual int nodeType => NodeConst.TypeUndefined;

    /* Init Node with given data or use default data */
    public void Init()
    {

    }

    /* Serialize the node into a binary stream */
    public virtual void Serialize(BinaryWriter writer)
    {
        return;
    }
}

public static class NodeStreamer
{
    /* Serialize node into byte stream */
    public static void Serialize(Node node, BinaryWriter writer)
    {
        node.Serialize(writer);
    }

    /* Deserialize bytes from reader into specific node */
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
}
