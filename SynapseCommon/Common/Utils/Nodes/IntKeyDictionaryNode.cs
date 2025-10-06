using System;
using System.Collections;
using System.Collections.Generic;

public class IntKeyDictionaryTemplateNodeCommon<T> : Node, IEnumerable<KeyValuePair<int, T>> where T : Node
{
    protected Dictionary<int, T> children = new Dictionary<int, T>();

    protected IntKeyDictionaryTemplateNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        params KeyValuePair<int, T>[] kvps
    ) : base(id_, nodeSyncType_)
    {
        foreach (KeyValuePair<int, T> kvp in kvps)
        {
            Add(kvp.Key, kvp.Value);
        }
    }

    protected string ChildrenToString()
    {
        string s = "";
        foreach (KeyValuePair<int, T> kvp in children.OrderBy(x => x.Key))
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
            int index = int.Parse(id_);
            if (TryGetValue(index, out T? child)) return child;
            else return null;
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
        foreach (KeyValuePair<int, T> kvp in children)
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
        foreach (KeyValuePair<int, T> kvp in children)
        {
            NodeStreamer.Serialize(new IntNode("", NodeSynConst.SyncAll, kvp.Key), writer);
            NodeStreamer.Serialize(kvp.Value, writer);
        }
        NodeStreamer.Serialize(new IntNode("", NodeSynConst.SyncAll), writer);
        NodeStreamer.Serialize(new IntKeyDictionaryTailNode("", NodeSynConst.SyncAll), writer);
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
            T valueCopy = (T)value.Copy();
            children[key] = valueCopy;
            valueCopy.SetId($"{key}");
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
        T valueCopy = (T)value.Copy();
        children.Add(key, valueCopy);
        valueCopy.SetId($"{key}");
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
    protected IntKeyDictionaryNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        params KeyValuePair<int, Node>[] kvps
    ) : base(id_, nodeSyncType_, kvps) { }

    public override string ToString()
    {
        return $"{this.GetType().Name}({{{ChildrenToString()}}})";
    }

    #region REGION_IDENTIFICATION

    public virtual object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        foreach (KeyValuePair<int, Node> kvp in children)
        {
            argsList.Add(kvp);
        }
        return argsList.ToArray();
    }

    #endregion
}

#if DEBUG

[RegisterTest]
public static class TestIntKeyDictionaryNode
{
    public static void TestStream()
    {
        IntKeyDictionaryNode node = new IntKeyDictionaryNode(
            "", NodeSynConst.SyncAll,
            new KeyValuePair<int, Node>(0, new IntNode("", NodeSynConst.SyncAll, 3)),
            new KeyValuePair<int, Node>(1, new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
            new KeyValuePair<int, Node>(2, new StringNode("", NodeSynConst.SyncAll, "3")),
            new KeyValuePair<int, Node>(3, new ListNode(
                "", NodeSynConst.SyncAll,
                new IntNode("", NodeSynConst.SyncAll, 4),
                new FloatNode("", NodeSynConst.SyncAll, 5.0f)
            )),
            new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode(
                "", NodeSynConst.SyncAll,
                new KeyValuePair<int, Node>(0, new IntNode("", NodeSynConst.SyncAll, 3)),
                new KeyValuePair<int, Node>(1, new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
                new KeyValuePair<int, Node>(2, new StringNode("", NodeSynConst.SyncAll, "3")),
                new KeyValuePair<int, Node>(3, new ListNode(
                    "", NodeSynConst.SyncAll,
                    new IntNode("", NodeSynConst.SyncAll, 4),
                    new FloatNode("", NodeSynConst.SyncAll, 5.0f)
                )),
                new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode("", NodeSynConst.SyncAll))
            ))
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "IntKeyDictionaryNode changed after serialization and deserialization");
    }
    
    public static void TestCopy()
    {
        IntKeyDictionaryNode node = new IntKeyDictionaryNode(
            "", NodeSynConst.SyncAll,
            new KeyValuePair<int, Node>(0, new IntNode("", NodeSynConst.SyncAll, 3)),
            new KeyValuePair<int, Node>(1, new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
            new KeyValuePair<int, Node>(2, new StringNode("", NodeSynConst.SyncAll, "3")),
            new KeyValuePair<int, Node>(3, new ListNode(
                "", NodeSynConst.SyncAll,
                new IntNode("", NodeSynConst.SyncAll, 4),
                new FloatNode("", NodeSynConst.SyncAll, 5.0f)
            )),
            new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode(
                "", NodeSynConst.SyncAll,
                new KeyValuePair<int, Node>(0, new IntNode("", NodeSynConst.SyncAll, 3)),
                new KeyValuePair<int, Node>(1, new FloatNode("", NodeSynConst.SyncAll, 3.3f)),
                new KeyValuePair<int, Node>(2, new StringNode("", NodeSynConst.SyncAll, "3")),
                new KeyValuePair<int, Node>(3, new ListNode(
                    "", NodeSynConst.SyncAll,
                    new IntNode("", NodeSynConst.SyncAll, 4),
                    new FloatNode("", NodeSynConst.SyncAll, 5.0f)
                )),
                new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode("", NodeSynConst.SyncAll))
            ))
        );
        IntKeyDictionaryNode copy = (IntKeyDictionaryNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "IntKeyDictionaryNode id not equal after copy");
    }
}

#endif
