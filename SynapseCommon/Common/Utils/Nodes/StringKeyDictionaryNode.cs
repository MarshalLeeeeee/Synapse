using System;
using System.Collections;
using System.Collections.Generic;

public class StringKeyDictionaryTemplateNodeCommon<T> : Node, IEnumerable<KeyValuePair<string, T>> where T : Node
{
    protected Dictionary<string, T> children = new Dictionary<string, T>();

    protected StringKeyDictionaryTemplateNodeCommon(params KeyValuePair<string, T>[] kvps)
    {
        foreach (KeyValuePair<string, T> kvp in kvps)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    protected string ChildrenToString()
    {
        string s = "";
        foreach (KeyValuePair<string, T> kvp in children)
        {
            s += $"{kvp.Key}:{kvp.Value}, ";
        }
        return s;
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        foreach (KeyValuePair<string, T> kvp in children)
        {
            NodeStreamer.Serialize(new StringNode(kvp.Key), writer);
            NodeStreamer.Serialize(kvp.Value, writer);
        }
        NodeStreamer.Serialize(new StringNode(), writer);
        NodeStreamer.Serialize(new StringKeyDictionaryTailNode(), writer);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        while (true)
        {
            Node keyNode = NodeStreamer.Deserialize(reader);
            Node valueNode = NodeStreamer.Deserialize(reader);
            if (keyNode is StringNode stringKeyNode)
            {
                if (valueNode is StringKeyDictionaryTailNode) break;
                if (valueNode is T tNode)
                {
                    argsList.Add(new KeyValuePair<string, T>(stringKeyNode.Get(), tNode));
                }
                else
                {
                    throw new InvalidDataException("Failed to deserialize key node of StringKeyDictionaryNode.");
                }
            }
            else
            {
                throw new InvalidDataException("Failed to deserialize key node of StringKeyDictionaryNode.");
            }
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public T this[string key]
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

    public bool ContainsKey(string key)
    {
        return children.ContainsKey(key);
    }

    public bool TryGetValue(string key, out T? value)
    {
        return children.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(string key, T value)
    {
        children.Add(key, value);
    }

    public void Remove(string key)
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

public class StringKeyDictionaryNodeCommon : StringKeyDictionaryTemplateNodeCommon<Node>
{
    protected StringKeyDictionaryNodeCommon(params KeyValuePair<string, Node>[] kvps) : base(kvps) { }

    public override string ToString()
    {
        return $"StringKeyDictionaryNode({{{ChildrenToString()}}})";
    }
}
