/// <summary>
/// Entity
/// <para> Entity is synchronizable. </para>
/// <para> Entity holds data and function of a functional game instance. </para>
/// <para> Entity holds components as an implementation of moduled functionality. </para>
/// </summary>

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterEntityAttribute : Attribute
{
    public RegisterEntityAttribute() { }
}

public class EntityCommon : Node
{
    /// <summary>
    /// manage components, component name -> component instance
    /// </summary>
    protected Components components = new Components("components", NodeSynConst.SyncAll);

    protected EntityCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        Components? components_ = null
    ) : base(id_, nodeSyncType_)
    {
        if (String.IsNullOrEmpty(id))
        {
            id = "Ett-" + Guid.NewGuid().ToString();
        }

        if (components_ != null)
        {
            components = (Components)components_.Copy();
        }
        InitComponents();
    }

    protected virtual void InitComponents() { }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        argsList.Add(components);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(id);
        writer.Write(nodeSyncType);
        NodeStreamer.Serialize(components, writer);
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
        argsList.Add(NodeStreamer.Deserialize(reader));
        return argsList.ToArray();
    }

    #endregion
}