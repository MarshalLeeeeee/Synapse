/// <summary>
/// Component
/// <para> Component is owned by Entity. </para>
/// <para> Component is synchronizable. </para>
/// <para> Component holds functionality of an Entity, which is prone to be designed to be decoupled and reusable. </para>
/// </summary>

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterComponent : Attribute
{
    public RegisterComponent() { }
}

public class ComponentCommon : Node
{
    protected ComponentCommon() : base() { }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer, string proxyId)
    {
        writer.Write(nodeType);
    }

    /// <summary>
    /// Collect arguments for constructor from binary reader.
    /// </summary>
    /// <returns> List of arguments for constructor </returns>
    protected static object[] DeserializeIntoArgs(BinaryReader reader)
    {
        List<object> argsList = new List<object>();
        return argsList.ToArray();
    }

    #endregion
}

public class ComponentsCommon : StringKeyDictionaryTemplateNodeCommon<Component>
{
    protected ComponentsCommon(
        params KeyValuePair<string, Component>[] kvps
    ) : base(kvps) { }
}
