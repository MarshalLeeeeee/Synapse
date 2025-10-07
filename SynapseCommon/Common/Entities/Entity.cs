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
        Components? components_ = null
    ) : base()
    {
        if (components_ != null)
        {
            components = (Components)components_.Copy();
        }
        InitComponents();
    }

    protected virtual void InitComponents() { }

    #region REGION_IDENTIFICATION

    public override void SetId(string id_, Node? parent_ = null)
    {
        base.SetId(id_, parent_);
        components.SetId("components", this);
    }

    public override Node? GetChildWithId(string id_)
    {
        if (id_ == "components") return components;
        return base.GetChildWithId(id_);
    }

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(components.Copy());
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
        NodeStreamer.Serialize(components, writer, proxyId);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        argsList.Add(NodeStreamer.Deserialize(reader));
        return argsList.ToArray();
    }

    #endregion
}