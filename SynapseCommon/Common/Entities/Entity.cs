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

    /// <summary>
    /// manage components, component name -> component instance
    /// </summary>
    protected Components components = new Components();

    protected EntityCommon(string entityId_ = "", Components? components_ = null)
    {
        if (String.IsNullOrEmpty(entityId_))
        {
            entityId = Guid.NewGuid().ToString();
        }
        else
        {
            entityId = entityId_;
        }

        if (components_ != null)
        {
            components = components_;
        }
    }
}