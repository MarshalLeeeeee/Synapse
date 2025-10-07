using System;
using System.Collections;
using System.Collections.Generic;

public class IntKeyDictionaryTemplateNodeCommon<T> : Node, IEnumerable<KeyValuePair<int, T>> where T : Node
{
    protected Dictionary<int, T> children = new Dictionary<int, T>();

    protected IntKeyDictionaryTemplateNodeCommon(
        params KeyValuePair<int, T>[] kvps
    ) : base()
    {
        foreach (KeyValuePair<int, T> kvp in kvps)
        {
            T valueCopy = (T)kvp.Value.Copy();
            children[kvp.Key] = valueCopy;
            valueCopy.SetId($"{kvp.Key}", this);
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

    public override string ToString()
    {
        return $"{this.GetType().Name}({{ {ChildrenToString()} }})";
    }

    #region REGION_IDENTIFICATION

    public override void SetId(string id_, Node? parent_ = null)
    {
        base.SetId(id_, parent_);
        foreach (KeyValuePair<int, T> kvp in children)
        {
            kvp.Value.SetId($"{kvp.Key}", this);
        }
    }

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
        foreach (KeyValuePair<int, T> kvp in children)
        {
            argsList.Add(new KeyValuePair<int, T>(kvp.Key, (T)kvp.Value.Copy()));
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        foreach (KeyValuePair<int, T> kvp in children)
        {
            if (kvp.Value.ShouldSerializeContent(proxyId))
            {
                NodeStreamer.Serialize(new IntNode(kvp.Key), writer, proxyId);
                NodeStreamer.Serialize(kvp.Value, writer, proxyId);
            }
        }
        NodeStreamer.Serialize(new IntNode(), writer, proxyId);
        NodeStreamer.Serialize(new IntKeyDictionaryTailNode(), writer, proxyId);
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
            T valueCopy = (T)value.Copy();
            children[key] = valueCopy;
            valueCopy.SetId($"{key}", this);
            OnSet(key, valueCopy);
        }
    }

    protected virtual void OnSet(int key, T value) { }

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
        valueCopy.SetId($"{key}", this);
        OnAdd(key, valueCopy);
    }

    protected virtual void OnAdd(int key, T value) { }

    public void Remove(int key)
    {
        if (children.TryGetValue(key, out T? value) && value != null)
        {
            children.Remove(key);
            OnRemove(key);
        }
    }

    protected virtual void OnRemove(int key) { }

    public void Clear()
    {
        children.Clear();
        OnClear();
    }

    protected virtual void OnClear() { }

    #endregion
}


public class IntKeyDictionaryNodeCommon : IntKeyDictionaryTemplateNodeCommon<Node>
{
    protected IntKeyDictionaryNodeCommon(
        params KeyValuePair<int, Node>[] kvps
    ) : base(kvps) { }
}

#if DEBUG

[RegisterTest]
public static class TestIntKeyDictionaryNode
{
    public static void TestStream()
    {
        IntKeyDictionaryNode node = new IntKeyDictionaryNode(
            new KeyValuePair<int, Node>(0, new IntNode(3)),
            new KeyValuePair<int, Node>(1, new FloatNode(3.3f)),
            new KeyValuePair<int, Node>(2, new StringNode("3")),
            new KeyValuePair<int, Node>(3, new ListNode(
                new IntNode(4),
                new FloatNode(5.0f)
            )),
            new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode(
                new KeyValuePair<int, Node>(0, new IntNode(3)),
                new KeyValuePair<int, Node>(1, new FloatNode(3.3f)),
                new KeyValuePair<int, Node>(2, new StringNode("3")),
                new KeyValuePair<int, Node>(3, new ListNode(
                    new IntNode(4),
                    new FloatNode(5.0f)
                )),
                new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode())
            ))
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "IntKeyDictionaryNode changed after serialization and deserialization");
    }
    
    public static void TestCopy()
    {
        IntKeyDictionaryNode node = new IntKeyDictionaryNode(
            new KeyValuePair<int, Node>(0, new IntNode(3)),
            new KeyValuePair<int, Node>(1, new FloatNode(3.3f)),
            new KeyValuePair<int, Node>(2, new StringNode("3")),
            new KeyValuePair<int, Node>(3, new ListNode(
                new IntNode(4),
                new FloatNode(5.0f)
            )),
            new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode(
                new KeyValuePair<int, Node>(0, new IntNode(3)),
                new KeyValuePair<int, Node>(1, new FloatNode(3.3f)),
                new KeyValuePair<int, Node>(2, new StringNode("3")),
                new KeyValuePair<int, Node>(3, new ListNode(
                    new IntNode(4),
                    new FloatNode(5.0f)
                )),
                new KeyValuePair<int, Node>(4, new IntKeyDictionaryNode())
            ))
        );
        IntKeyDictionaryNode copy = (IntKeyDictionaryNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "IntKeyDictionaryNode id not equal after copy");
    }
}

#endif
