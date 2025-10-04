using System.Collections;

public class ListTemplateNodeCommon<T> : Node, IEnumerable<T> where T : Node
{
    protected List<T> children = new List<T>();

    protected ListTemplateNodeCommon(params T[] nodes)
    {
        foreach (T node in nodes)
        {
            Add(node);
        }
    }

    protected string ChildrenToString()
    {
        string s = "";
        foreach (T node in children)
        {
            s += $"{node}, ";
        }
        return s;
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        foreach (T child in children)
        {
            NodeStreamer.Serialize(child, writer);
        }
        NodeStreamer.Serialize(new ListTailNode(), writer);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        while (true)
        {
            Node node = NodeStreamer.Deserialize(reader);
            if (node is ListTailNode) break;
            if (node is T tNode)
            {
                argsList.Add(node);
            }
            else
            {
                throw new InvalidDataException($"Failed to deserialize key node of ListTemplateNodeCommon<{typeof(T).Name}>.");
            }
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

    public T this[int index]
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

    public bool Contains(T child)
    {
        return children.Contains(child);
    }

    public int IndexOf(T child)
    {
        return children.IndexOf(child);
    }

    public T[] ToArray()
    {
        return children.ToArray();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T child)
    {
        children.Add(child);
    }

    public void Insert(int index, T child)
    {
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.Insert(index, child);
    }

    public void Remove(T child)
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
}

public class ListNodeCommon : ListTemplateNodeCommon<Node>
{
    protected ListNodeCommon(params Node[] nodes) : base(nodes) { }

    public override string ToString()
    {
        return $"ListNode([{ChildrenToString()}])";
    }
}

#if DEBUG

[RegisterTest]
public static class TestListNode
{
    public static void TestStream()
    {
        ListNode node = new ListNode(new IntNode(1), new FloatNode(10.0f), new BoolNode(true), new StringNode("lmc"), 
            new ListNode(new IntNode(1), new FloatNode(10.0f), new BoolNode(true), new StringNode("lmc")));
        Assert.EqualTrue(NodeStreamer.TestStream(node), "ListNode changed after serialization and deserialization");
    }
}

#endif
