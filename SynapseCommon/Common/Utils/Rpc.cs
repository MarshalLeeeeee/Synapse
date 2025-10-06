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
    public int[] argTypes { get; }

    public RpcAttribute(int rpcType_, params int[] argTypes_)
    {
        rpcType = rpcType_;
        argTypes = argTypes_;
    }
}

public class RpcMethodInfo
{
    public MethodInfo mehodInfo;
    public int rpcType;
    public int[] argTypes;

    public RpcMethodInfo(MethodInfo methodInfo_, int rpcType_, int[] argTypes_)
    {
        mehodInfo = methodInfo_;
        rpcType = rpcType_;
        argTypes = argTypes_;
    }

    public bool CheckArgTypes(List<Node> args)
    {
        int argTypesCount = argTypes.Length;
        int argsCount = args.Count;
        if (argTypesCount != argsCount) return false;

        int i = 0;
        while (i < argTypesCount)
        {
            int argType = argTypes[i];
            Node arg = args[i];
            if (argType != NodeTypeConst.TypeUndefined && arg.nodeType != argTypes[i]) return false;
            i++;
        }
        return true;
    }

    public void Invoke(object instance, object[] methodArgs)
    {
        mehodInfo.Invoke(instance, methodArgs);
    }
}

