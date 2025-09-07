/*
 * Entity is synchronizable.
 * Entity holds data and function of a functional game instance.
 * Entity holds components as an implementation of moduled functionality.
 */

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterEntity : Attribute
{
    public RegisterEntity() { }
}

public class Entity : Node
{

}