/// <summary>
/// Manager
/// <para> Manager is owned by Game. </para>
/// </summary>

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterManagerAttribute : Attribute
{
    public RegisterManagerAttribute() { }
}

public class Manager
{
    public void Start()
    {
        OnStart();
    }

    protected virtual void OnStart() { }

    public void Update(float dt)
    {
        DoUpdate(dt);
    }

    protected virtual void DoUpdate(float dt) { }

    public void Destroy()
    {
        OnDestroy();
    }

    protected virtual void OnDestroy() { }
}