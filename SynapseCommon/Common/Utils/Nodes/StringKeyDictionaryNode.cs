using System;
using System.Collections;
using System.Collections.Generic;

public class StringKeyDictionaryTemplateNodeCommon<T> : Node, IEnumerable<KeyValuePair<string, T>> where T : Node
{
    protected Dictionary<string, T> children = new Dictionary<string, T>();

    protected StringKeyDictionaryTemplateNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        params KeyValuePair<string, T>[] kvps
    ) : base(id_, nodeSyncType_)
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
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        foreach (KeyValuePair<string, T> kvp in children)
        {
            argsList.Add(kvp);
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(id);
        writer.Write(nodeSyncType);
        foreach (KeyValuePair<string, T> kvp in children)
        {
            NodeStreamer.Serialize(new StringNode("", NodeSynConst.SyncAll, kvp.Key), writer);
            NodeStreamer.Serialize(kvp.Value, writer);
        }
        NodeStreamer.Serialize(new StringNode("", NodeSynConst.SyncAll), writer);
        NodeStreamer.Serialize(new StringKeyDictionaryTailNode("", NodeSynConst.SyncAll), writer);
    }

    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
        argsList.Add(reader.ReadInt32());
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
    protected StringKeyDictionaryNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll, 
        params KeyValuePair<string, Node>[] kvps
    ) : base(id_, nodeSyncType_, kvps) { }

    public override string ToString()
    {
        return $"{this.GetType().Name}({{{ChildrenToString()}}})";
    }
}

#if DEBUG

[RegisterTest]
public static class TestStringKeyDictionaryNode
{
    public static void TestStream()
    {
        StringKeyDictionaryNode node = new StringKeyDictionaryNode(
            "", NodeSynConst.SyncAll,
            new KeyValuePair<string, Node>("0", new IntNode("", NodeSynConst.SyncAll, 3)),
            new KeyValuePair<string, Node>("1", new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
            new KeyValuePair<string, Node>("2", new StringNode("", NodeSynConst.SyncAll, "3")),
            new KeyValuePair<string, Node>("3", new ListNode(
                "", NodeSynConst.SyncAll,
                new IntNode("", NodeSynConst.SyncAll, 4),
                new FloatNode("", NodeSynConst.SyncAll, 5.0f)
            )),
            new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode(
                "", NodeSynConst.SyncAll,
                new KeyValuePair<string, Node>("0", new IntNode("", NodeSynConst.SyncAll, 3)),
                new KeyValuePair<string, Node>("1", new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
                new KeyValuePair<string, Node>("2", new StringNode("", NodeSynConst.SyncAll, "3")),
                new KeyValuePair<string, Node>("3", new ListNode(
                    "", NodeSynConst.SyncAll,
                    new IntNode("", NodeSynConst.SyncAll, 4),
                    new FloatNode("", NodeSynConst.SyncAll, 5.0f)
                )),
                new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode("", NodeSynConst.SyncAll))
            ))
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "StringKeyDictionaryNode changed after serialization and deserialization");
    }
    
    public static void TestCopy()
    {
        StringKeyDictionaryNode node = new StringKeyDictionaryNode(
            "", NodeSynConst.SyncAll,
            new KeyValuePair<string, Node>("0", new IntNode("", NodeSynConst.SyncAll, 3)),
            new KeyValuePair<string, Node>("1", new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
            new KeyValuePair<string, Node>("2", new StringNode("", NodeSynConst.SyncAll, "3")),
            new KeyValuePair<string, Node>("3", new ListNode(
                "", NodeSynConst.SyncAll,
                new IntNode("", NodeSynConst.SyncAll, 4),
                new FloatNode("", NodeSynConst.SyncAll, 5.0f)
            )),
            new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode(
                "", NodeSynConst.SyncAll,
                new KeyValuePair<string, Node>("0", new IntNode("", NodeSynConst.SyncAll, 3)),
                new KeyValuePair<string, Node>("1", new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
                new KeyValuePair<string, Node>("2", new StringNode("", NodeSynConst.SyncAll, "3")),
                new KeyValuePair<string, Node>("3", new ListNode(
                    "", NodeSynConst.SyncAll,
                    new IntNode("", NodeSynConst.SyncAll, 4),
                    new FloatNode("", NodeSynConst.SyncAll, 5.0f)
                )),
                new KeyValuePair<string, Node>("4", new StringKeyDictionaryNode("", NodeSynConst.SyncAll))
            ))
        );
        StringKeyDictionaryNode copy = (StringKeyDictionaryNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "StringKeyDictionaryNode id not equal after copy");
    }
}

#endif
