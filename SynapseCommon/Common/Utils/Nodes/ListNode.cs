using System.Collections;

public class ListNodeCommon : Node, IEnumerable<Node>
{
    protected List<Node> children = new List<Node>();

    public ListNodeCommon(params Node[] nodes)
    {
        foreach (Node node in nodes)
        {
            Add(node);
        }
    }

    public override string ToString()
    {
        string s = "";
        foreach (Node node in children)
        {
            s += $"{node}, ";
        }
        return $"ListNode([{s}])";
    }

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(NodeConst.TypeList);
        foreach (Node child in children)
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
            else argsList.Add(node);
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_API

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
