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
    protected Components components = new Components();

    protected EntityCommon(
        string id_ = "",
        Components? components_ = null
    ) : base(id_)
    {
        if (components_ != null)
        {
            components = (Components)components_.Copy();
        }
        InitComponents();
    }

    protected virtual void InitComponents() { }

    #region REGION_IDENTIFICATION

    public override void SetId(string id_)
    {
        base.SetId(id_);
        components.SetId(id_ + ".components");
    }

    public override Node? GetChildWithId(string id_)
    {
        if (id_ == "components") return components;
        return base.GetChildWithId(id_);
    }

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add("");
        argsList.Add(components);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        writer.Write(id);
        NodeStreamer.Serialize(components, writer, proxyId);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(reader.ReadString());
        argsList.Add(NodeStreamer.Deserialize(reader));
        return argsList.ToArray();
    }

    #endregion
}