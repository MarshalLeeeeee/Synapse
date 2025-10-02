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

public class Component : Node
{

}