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
    /// exclusive id for entity
    /// </summary>
    public string entityId { get; protected set; } = "";

    protected EntityCommon(string eid = "")
    {
        if (String.IsNullOrEmpty(eid))
        {
            entityId = Guid.NewGuid().ToString();
        }
        else
        {
            entityId = eid;
        }
    }
}