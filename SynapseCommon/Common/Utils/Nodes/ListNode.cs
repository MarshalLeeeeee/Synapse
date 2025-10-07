using System.Collections;

public class ListTemplateNodeCommon<T> : Node, IEnumerable<T> where T : Node
{
    protected List<T> children = new List<T>();

    protected ListTemplateNodeCommon(
        params T[] nodes
    ) : base()
    {
        int i = 0;
        foreach (T node in nodes)
        {
            T nodeCopy = (T)node.Copy();
            children.Add(nodeCopy);
            nodeCopy.SetId($"{i}", this);
            i += 1;
        }
    }

    protected string ChildrenToString()
    {
        string s = "";
        foreach (T child in children)
        {
            s += $"{child}, ";
        }
        return s;
    }

    public override string ToString()
    {
        return $"{this.GetType().Name}([{ChildrenToString()}])";
    }

    #region REGION_IDENTIFICATION

    public override void SetId(string id_, Node? parent_ = null)
    {
        base.SetId(id_, parent_);
        UpdateChildrenId();
    }

    protected void UpdateChildrenId()
    {
        int i = 0;
        foreach (T child in children)
        {
            child.SetId($"{i}", this);
            i += 1;
        }
    }

    public override Node? GetChildWithId(string id_)
    {
        try
        {
            int index = int.Parse(id_);
            if (index < 0 || index >= Count) return null;
            else return this[index];
        }
        catch (Exception)
        {
            return null;
        }
    }

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        foreach (T child in children)
        {
            argsList.Add(child.Copy());
        }
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        foreach (T child in children)
        {
            NodeStreamer.Serialize(child, writer, proxyId);
        }
        NodeStreamer.Serialize(new ListTailNode(), writer, proxyId);
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
            T valueCopy = (T)value.Copy();
            children[index] = valueCopy;
            valueCopy.SetId($"{index}", this);
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
        T childCopy = (T)child.Copy();
        children.Add(childCopy);
        childCopy.SetId($"{Count-1}", this);
        OnAdd(childCopy);
    }

    protected virtual void OnAdd(T child) {}

    public void Insert(int index, T child)
    {
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        T childCopy = (T)child.Copy();
        children.Insert(index, childCopy);
        UpdateChildrenId();
        OnInsert(index, childCopy);
    }

    protected virtual void OnInsert(int index, T child) {}

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.RemoveAt(index);
        UpdateChildrenId();
        OnRemoveAt(index);
    }

    protected virtual void OnRemoveAt(int index) {}

    public void Clear()
    {
        children.Clear();
        OnClear();
    }

    protected virtual void OnClear() {}

    #endregion
}

public class ListNodeCommon : ListTemplateNodeCommon<Node>
{
    protected ListNodeCommon(
        params Node[] nodes
    ) : base(nodes) { }
}

#if DEBUG

[RegisterTest]
public static class TestListNode
{
    public static void TestStream()
    {
        ListNode node = new ListNode(
            new IntNode(1),
            new FloatNode(10.0f),
            new BoolNode(true),
            new StringNode("lmc"),
            new ListNode(
                new IntNode(1),
                new FloatNode(10.0f),
                new BoolNode(true),
                new StringNode("lmc")
            )
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "ListNode changed after serialization and deserialization");
    }
    
    public static void TestCopy()
    {
        ListNode node = new ListNode(
            new IntNode(1),
            new FloatNode(10.0f),
            new BoolNode(true),
            new StringNode("lmc"),
            new ListNode(
                new IntNode(1),
                new FloatNode(10.0f),
                new BoolNode(true),
                new StringNode("lmc")
            )
        );
        ListNode copy = (ListNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "ListNode id not equal after copy");
    }
}

#endif
