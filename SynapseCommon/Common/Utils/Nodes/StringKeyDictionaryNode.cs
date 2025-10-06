using System;
using System.Collections;
using System.Collections.Generic;

public class StringKeyDictionaryTemplateNodeCommon<T> : Node, IEnumerable<KeyValuePair<string, T>> where T : Node
{
    protected Dictionary<string, T> children = new Dictionary<string, T>();

    protected StringKeyDictionaryTemplateNodeCommon(
        string id_ = "",
        params KeyValuePair<string, T>[] kvps
    ) : base(id_)
    {
        foreach (KeyValuePair<string, T> kvp in kvps)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    protected string ChildrenToString()
    {
        string s = "";
        foreach (KeyValuePair<string, T> kvp in children.OrderBy(x => x.Key))
        {
            s += $"{kvp.Key}:{kvp.Value}, ";
        }
        return s;
    }

    public override string ToString()
    {
        return $"{this.GetType().Name}({{ {ChildrenToString()} }})";
    }

    #region REGION_IDENTIFICATION

    public override Node? GetChildWithId(string id_)
    {
        try
        {
            if (!children.ContainsKey(id_)) return null;
            return children[id_];
        }
        catch (Exception)
        {
            return null;
        }
    }

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add("");
        foreach (KeyValuePair<string, T> kvp in children)
        {
            argsList.Add(kvp);
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        writer.Write(id);
        foreach (KeyValuePair<string, T> kvp in children)
        {
            if (kvp.Value.ShouldSerializeContent(proxyId))
            {
                NodeStreamer.Serialize(new StringNode("", kvp.Key), writer, proxyId);
                NodeStreamer.Serialize(kvp.Value, writer, proxyId);
            }
        }
        NodeStreamer.Serialize(new StringNode(), writer, proxyId);
        NodeStreamer.Serialize(new StringKeyDictionaryTailNode(), writer, proxyId);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
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
            T valueCopy = (T)value.Copy();
            children[key] = valueCopy;
            valueCopy.SetId($"{id}.{key}");
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
        T valueCopy = (T)value.Copy();
        children.Add(key, valueCopy);
        valueCopy.SetId($"{id}.{key}");
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
    protected StringKeyDictionaryNodeCommon(
        string id_ = "",
        params KeyValuePair<string, Node>[] kvps
    ) : base(id_, kvps) { }
}

#if DEBUG

[RegisterTest]
public static class TestStringKeyDictionaryNode
{
    public static void TestStream()
    {
        StringKeyDictionaryNode node = new StringKeyDictionaryNode(
            "", 
            new KeyValuePair<string, Node>("0", new IntNode("", 3)),
            new KeyValuePair<string, Node>("1", new FloatNode("", 3.3f)),
            new KeyValuePair<string, Node>("2", new StringNode("", "3")),
            new KeyValuePair<string, Node>("3", new ListNode(
                "", 
                new IntNode("", 4),
                new FloatNode("", 5.0f)
            )),
            new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode(
                "", 
                new KeyValuePair<string, Node>("0", new IntNode("", 3)),
                new KeyValuePair<string, Node>("1", new FloatNode("", 3.3f)),
                new KeyValuePair<string, Node>("2", new StringNode("", "3")),
                new KeyValuePair<string, Node>("3", new ListNode(
                    "", 
                    new IntNode("", 4),
                    new FloatNode("", 5.0f)
                )),
                new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode())
            ))
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "StringKeyDictionaryNode changed after serialization and deserialization");
    }
    
    public static void TestCopy()
    {
        StringKeyDictionaryNode node = new StringKeyDictionaryNode(
            "", 
            new KeyValuePair<string, Node>("0", new IntNode("", 3)),
            new KeyValuePair<string, Node>("1", new FloatNode("", 3.3f)),
            new KeyValuePair<string, Node>("2", new StringNode("", "3")),
            new KeyValuePair<string, Node>("3", new ListNode(
                "", 
                new IntNode("", 4),
                new FloatNode("", 5.0f)
            )),
            new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode(
                "", 
                new KeyValuePair<string, Node>("0", new IntNode("", 3)),
                new KeyValuePair<string, Node>("1", new FloatNode("", 3.3f)),
                new KeyValuePair<string, Node>("2", new StringNode("", "3")),
                new KeyValuePair<string, Node>("3", new ListNode(
                    "", 
                    new IntNode("", 4),
                    new FloatNode("", 5.0f)
                )),
                new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode())
            ))
        );
        StringKeyDictionaryNode copy = (StringKeyDictionaryNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "StringKeyDictionaryNode id not equal after copy");
    }
}

#endif
