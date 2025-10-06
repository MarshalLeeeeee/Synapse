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
    protected ComponentCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll
    ) : base(id_, nodeSyncType_) { }

    #region REGION_IDENTIFICATION

    public override object[] GetCopyArgs()
    {
        List<object> argsList = new List<object>();
        argsList.Add(id);
        argsList.Add(nodeSyncType);
        return argsList.ToArray();
    }

    #endregion

    #region REGION_STREAM

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(nodeType);
        writer.Write(id);
        writer.Write(nodeSyncType);
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
        return argsList.ToArray();
    }

    #endregion
}

public class ComponentsCommon : StringKeyDictionaryTemplateNodeCommon<Component>
{
    protected ComponentsCommon(
        string id_ = "", int nodeSyncType_ = NodeSynConst.SyncAll,
        params KeyValuePair<string, Component>[] kvps
    ) : base(id_, nodeSyncType_, kvps) { }

    public override string ToString()
    {
        return $"{this.GetType().Name}({{{ChildrenToString()}}})";
    }
}
