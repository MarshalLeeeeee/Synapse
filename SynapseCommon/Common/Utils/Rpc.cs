using System.Reflection;

public class RpcConst
{
    public const int OwnClient = (1 << 0);
    public const int AnyClient = (1 << 1);
    public const int Server = (1 << 2);

    public const int Client = OwnClient | AnyClient;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class RpcAttribute : Attribute
{
    public int rpcType { get; }

    public RpcAttribute(int rpcType_)
    {
        rpcType = rpcType_;
    }
}

public class RpcMethodInfo
{
    public MethodInfo mehodInfo;
    public int rpcType;

    public RpcMethodInfo(MethodInfo methodInfo_, int rpcType_)
    {
        mehodInfo = methodInfo_;
        rpcType = rpcType_;
    }

    public void Invoke(object instance, object[] methodArgs)
    {
        mehodInfo.Invoke(instance, methodArgs);
    }
}

