/// <summary>
/// Entity
/// <para> Entity is synchronizable. </para>
/// <para> Entity holds data and function of a functional game instance. </para>
/// <para> Entity holds components as an implementation of moduled functionality. </para>
/// </summary>

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterEntity : Attribute
{
    public RegisterEntity() { }
}

public class Entity : Node
{

}