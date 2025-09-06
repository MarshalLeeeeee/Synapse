/*
 * Component is owned by Entity.
 * Component is synchronizable.
 * Component holds functionality of an Entity, which is prone to be designed to be decoupled and reusable.
 */

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class IsComponent : Attribute
{
    public IsComponent() { }
}

public class Component : Node
{

}