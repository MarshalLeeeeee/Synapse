/*
 * Basic node of the game tree
 * Node can be serialized and synchronized from server to client
 * Depending on different client, node can be synchronized differently and partially
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

public class Node
{
    /* Every node has a unique id at runtime，
     * which serves as the identifier of the node in both server and client. 
     */
    public string id = "";

    /* static data type */
    public static int staticNodeType = NodeConst.TypeUndefined;
    /* dynamic data type */
    public virtual int nodeType => staticNodeType;

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

public class StringNode : Node
{
    /* static data type */
    public new static int staticNodeType = NodeConst.TypeString;
    /* dynamic data type */
    public override int nodeType => staticNodeType;
    /* data */
    private string s = "";

    public StringNode(string s_ = "") { s = s_; }

    public string Get()
    {
        return s;
    }

    public void Set(string s_)
    {
        if (s == s_) return;
        s = s_;
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeString);
        writer.Write(s);
    }

    public static StringNode Deserialize(BinaryReader reader)
    {
        try
        {
            string s_ = reader.ReadString();
            return new StringNode(s_);
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize StringNode.", ex);
        }
    }
}

public class ListTailNode : Node
{
    /* static data type */
    public new static int staticNodeType = NodeConst.TypeListTail;
    /* dynamic data type */
    public override int nodeType => staticNodeType;

    public override void Serialize(BinaryWriter writer)
    {
        return;
    }

    public static ListTailNode Deserialize(BinaryReader reader)
    {
        try
        {
            return new ListTailNode();
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize ListTailNode.", ex);
        }
    }
}

public class ListNode : Node, IEnumerable<Node>
{
    /* static data type */
    public new static int staticNodeType = NodeConst.TypeList;
    /* dynamic data type */
    public override int nodeType => staticNodeType;

    /* List data */
    private List<Node> children = new List<Node>();

    #region REGION_LIST_API

    public Node this[int index]
    {
        get
        {
            if (index < 0 || index >= children.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            return children[index];
        }
        set
        {
            if (index < 0 || index >= children.Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            Node oldValue = children[index];
            children[index] = value;
        }
    }

    public int Count
    {
        get { return children.Count; }
    }

    public bool Contains(Node child)
    {
        return children.Contains(child);
    }

    public int IndexOf(Node child)
    {
        return children.IndexOf(child);
    }

    public Node[] ToArray()
    {
        return children.ToArray();
    }

    public IEnumerator<Node> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Node child)
    {
        children.Add(child);
    }

    public void Insert(int index, Node child)
    {
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.Insert(index, child);
    }

    public void Remove(Node child)
    {
        children.Remove(child);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        Node child = children[index];
        children.RemoveAt(index);
    }

    public void Clear()
    {
        children.Clear();
    }

    #endregion

    /* Serialize the node into a binary stream */
    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeList);
        foreach (Node child in children)
        {
            child.Serialize(writer);
        }
        writer.Write(NodeConst.TypeListTail);
    }

    public static ListNode Deserialize(BinaryReader reader)
    {
        try
        {
            ListNode listNode = new ListNode();
            while (true)
            {
                Node node = NodeStreamer.Deserialize(reader);
                if (node is ListTailNode) break;
                else listNode.Add(node);
            }
            return listNode;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("Failed to deserialize ListNode.", ex);
        }
    }
}

public static class NodeStreamer
{
    public static byte[] Serialize(Node node)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        node.Serialize(writer);
        byte[] data = stream.ToArray();
        return data;
    }
    
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