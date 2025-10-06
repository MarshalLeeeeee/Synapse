using System.Collections;

public class ListTemplateNodeCommon<T> : Node, IEnumerable<T> where T : Node
{
    protected List<T> children = new List<T>();

    protected ListTemplateNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        params T[] nodes
    ) : base(id_, nodeSyncType_)
    {
        foreach (T node in nodes)
        {
            Add(node);
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

    #region REGION_IDENTIFICATION

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

    protected void UpdateChildrenId()
    {
        int i = 0;
        foreach (T child in children)
        {
            child.SetId($"{i}");
            i += 1;
        }
    }

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        foreach (T child in children)
        {
            argsList.Add(child);
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
        foreach (T child in children)
        {
            NodeStreamer.Serialize(child, writer);
        }
        NodeStreamer.Serialize(new ListTailNode("", NodeSynConst.SyncAll), writer);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
        argsList.Add(reader.ReadInt32());
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
            valueCopy.SetId($"{index}");
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
        childCopy.SetId($"{Count - 1}");
    }

    public void Insert(int index, T child)
    {
        if (index < 0 || index > children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        T childCopy = (T)child.Copy();
        children.Insert(index, childCopy);
        UpdateChildrenId();
    }

    public void Remove(T child)
    {
        children.Remove(child);
        UpdateChildrenId();
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= children.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
        children.RemoveAt(index);
        UpdateChildrenId();
    }

    public void Clear()
    {
        children.Clear();
    }

    #endregion
}

public class ListNodeCommon : ListTemplateNodeCommon<Node>
{
    protected ListNodeCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll, 
        params Node[] nodes
    ) : base(id_, nodeSyncType_, nodes) { }

    public override string ToString()
    {
        return $"{this.GetType().Name}([{ChildrenToString()}])";
    }
}

#if DEBUG

[RegisterTest]
public static class TestListNode
{
    public static void TestStream()
    {
        ListNode node = new ListNode(
            "", NodeSynConst.SyncAll,
            new IntNode("", NodeSynConst.SyncAll, 1),
            new FloatNode("", NodeSynConst.SyncAll, 10.0f),
            new BoolNode("", NodeSynConst.SyncAll, true),
            new StringNode("", NodeSynConst.SyncAll, "lmc"),
            new ListNode(
                "", NodeSynConst.SyncAll,
                new IntNode("", NodeSynConst.SyncAll, 1),
                new FloatNode("", NodeSynConst.SyncAll, 10.0f),
                new BoolNode("", NodeSynConst.SyncAll, true),
                new StringNode("", NodeSynConst.SyncAll, "lmc")
            )
        );
        Assert.EqualTrue(NodeStreamer.TestStream(node), "ListNode changed after serialization and deserialization");
    }
    
    public static void TestCopy()
    {
        ListNode node = new ListNode(
            "", NodeSynConst.SyncAll,
            new IntNode("", NodeSynConst.SyncAll, 1),
            new FloatNode("", NodeSynConst.SyncAll, 10.0f),
            new BoolNode("", NodeSynConst.SyncAll, true),
            new StringNode("", NodeSynConst.SyncAll, "lmc"),
            new ListNode(
                "", NodeSynConst.SyncAll,
                new IntNode("", NodeSynConst.SyncAll, 1),
                new FloatNode("", NodeSynConst.SyncAll, 10.0f),
                new BoolNode("", NodeSynConst.SyncAll, true),
                new StringNode("", NodeSynConst.SyncAll, "lmc")
            )
        );
        ListNode copy = (ListNode)node.Copy();
        Assert.EqualTrue($"{node}" == $"{copy}", "ListNode id not equal after copy");
    }
}

#endif
