using System;
using System.Collections;
using System.Collections.Generic;

public class StringKeyDictionaryNodeCommon : Node, IEnumerable<KeyValuePair<string, Node>>
{
    protected Dictionary<string, Node> children = new Dictionary<string, Node>();

    public StringKeyDictionaryNodeCommon(params KeyValuePair<string, Node>[] kvps)
    {
        foreach (KeyValuePair<string, Node> kvp in kvps)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    public override string ToString()
    {
        string s = "";
        foreach (KeyValuePair<string, Node> kvp in children)
        {
            s += $"{kvp.Key}:{kvp.Value}, ";
        }
        return $"StringKeyDictionaryNode({{{s}}})";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeTypeConst.TypeStringKeyDictionary);
        foreach (KeyValuePair<string, Node> kvp in children)
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
                argsList.Add(new KeyValuePair<string, Node>(stringKeyNode.Get(), valueNode));
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

    public Node this[string key]
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

    public bool TryGetValue(string key, out Node? value)
    {
        return children.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<string, Node>> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(string key, Node value)
    {
        children.Add(key, value);
    }

    public void Remove(string key)
    {
        if (children.TryGetValue(key, out Node? value) && value != null)
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
