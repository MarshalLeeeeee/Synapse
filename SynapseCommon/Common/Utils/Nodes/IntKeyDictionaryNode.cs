using System;
using System.Collections;
using System.Collections.Generic;

public class IntKeyDictionaryTemplateNodeCommon<T> : Node, IEnumerable<KeyValuePair<int, T>> where T : Node
{
    protected Dictionary<int, T> children = new Dictionary<int, T>();

    protected IntKeyDictionaryTemplateNodeCommon(params KeyValuePair<int, T>[] kvps)
    {
        foreach (KeyValuePair<int, T> kvp in kvps)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    protected string ChildrenToString()
    {
        string s = "";
        foreach (KeyValuePair<int, T> kvp in children)
        {
            s += $"{kvp.Key}:{kvp.Value}, ";
        }
        return s;
    }
    
    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        foreach (KeyValuePair<int, T> kvp in children)
        {
            NodeStreamer.Serialize(new IntNode(kvp.Key), writer);
            NodeStreamer.Serialize(kvp.Value, writer);
        }
        NodeStreamer.Serialize(new IntNode(), writer);
        NodeStreamer.Serialize(new IntKeyDictionaryTailNode(), writer);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        while (true)
        {
            Node keyNode = NodeStreamer.Deserialize(reader);
            Node valueNode = NodeStreamer.Deserialize(reader);
            if (keyNode is IntNode intKeyNode)
            {
                if (valueNode is IntKeyDictionaryTailNode) break;
                if (valueNode is T tNode)
                {
                    argsList.Add(new KeyValuePair<int, T>(intKeyNode.Get(), tNode));
                }
                else
                {
                    throw new InvalidDataException($"Failed to deserialize key node of IntKeyDictionaryTemplateNodeCommon<{typeof(T).Name}>.");
                }
            }
            else
            {
                throw new InvalidDataException($"Failed to deserialize key node of IntKeyDictionaryTemplateNodeCommon<{typeof(T).Name}>.");
            }
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public T this[int key]
    {
        get
        {
            if (!children.ContainsKey(key))
                throw new KeyNotFoundException($"Key ({key}) not found in dictionary.");
            return children[key];
        }
        set
        {
            children[key] = value;
        }
    }

    public int Count
    {
        get { return children.Count; }
    }

    public bool ContainsKey(int key)
    {
        return children.ContainsKey(key);
    }

    public bool TryGetValue(int key, out T? value)
    {
        return children.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(int key, T value)
    {
        children.Add(key, value);
    }

    public void Remove(int key)
    {
        if (children.TryGetValue(key, out T? value) && value != null)
        {
            children.Remove(key);
        }
    }

    public void Clear()
    {
        children.Clear();
    }

    #endregion
}


public class IntKeyDictionaryNodeCommon : IntKeyDictionaryTemplateNodeCommon<Node>
{
    protected IntKeyDictionaryNodeCommon(params KeyValuePair<int, Node>[] kvps) : base(kvps) { }

    public override string ToString()
    {
        return $"IntKeyDictionaryNode({{{ChildrenToString()}}})";
    }
}
