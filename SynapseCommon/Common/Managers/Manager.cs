/*
 * Manager is owned by Game.
 * Manager is the root of a synchronizable tree.
 */

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class IsManagerAttribute : Attribute
{
    public IsManagerAttribute() { }
}

public class Manager : Node
{
    public virtual void Start() { }
    public virtual void Update(float dt) { }
    public virtual void Destroy() { }
}