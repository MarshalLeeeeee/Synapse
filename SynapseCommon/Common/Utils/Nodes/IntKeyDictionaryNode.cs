using System;
using System.Collections;
using System.Collections.Generic;

public class IntKeyDictionaryNodeCommon : Node, IEnumerable<KeyValuePair<int, Node>>
{

    protected Dictionary<int, Node> children = new Dictionary<int, Node>();

    public IntKeyDictionaryNodeCommon(params KeyValuePair<int, Node>[] kvps)
    {
        foreach (KeyValuePair<int, Node> kvp in kvps)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    public override string ToString()
    {
        string s = "";
        foreach (KeyValuePair<int, Node> kvp in children)
        {
            s += $"{kvp.Key}:{kvp.Value}, ";
        }
        return $"IntKeyDictionaryNode({{{s}}})";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeIntKeyDictionary);
        foreach (KeyValuePair<int, Node> kvp in children)
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
                argsList.Add(new KeyValuePair<int, Node>(intKeyNode.Get(), valueNode));
            }
            else
            {
                throw new InvalidDataException("Failed to deserialize key node of IntKeyDictionaryNode.");
            }
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public Node this[int key]
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

    public bool TryGetValue(int key, out Node? value)
    {
        return children.TryGetValue(key, out value);
    }

    public IEnumerator<KeyValuePair<int, Node>> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(int key, Node value)
    {
        children.Add(key, value);
    }

    public void Remove(int key)
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
