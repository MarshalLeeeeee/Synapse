using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterTestAttribute : Attribute
{
    public RegisterTestAttribute() { }
}

public static class Assert
{
    public static void Equal<T>(T o1, T o2, string errMsg) where T : class
    {
        if (o1 != o2)
        {
            throw new ApplicationException(errMsg);
        }
    }

    public static void EqualTrue(bool cond, string errMsg)
    {
        if (!cond)
        {
            throw new ApplicationException(errMsg);
        }
    }

    public static void EqualFalse(bool cond, string errMsg)
    {
        if (cond)
        {
            throw new ApplicationException(errMsg);
        }
    }
}

