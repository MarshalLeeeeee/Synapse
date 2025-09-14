using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class IClientReflection : IReflection
{
    public bool GetRpcMethodInfo(MethodInfo method, int rpcType, out RpcMethodInfo? rpcMethodInfo)
    {
        RpcAttribute? rpcAttr = method.GetCustomAttribute<RpcAttribute>();
        if (rpcAttr == null || (rpcAttr.rpcType & rpcType) == 0)
        {
            rpcMethodInfo = null;
            return false;
        }

        ParameterInfo[] paramaters = method.GetParameters();
        List<int> rpcArgTypes = new List<int>();
        int paramIndex = 0;
        int paramCount = paramaters.Length;
        while (paramIndex < paramCount)
        {
            ParameterInfo param = paramaters[paramIndex];
            Type paramType = param.ParameterType;
            if (!paramType.IsSubclassOf(typeof(Node)) && paramType != typeof(Node))
            {
                rpcMethodInfo = null;
                return false;
            }
            object? value = paramType.GetField(
                "staticNodeType",
                BindingFlags.Static | BindingFlags.Public
            )?.GetValue(null);
            if (value != null && value is int paramNodeType)
            {
                rpcArgTypes.Add(paramNodeType);
            }
            else
            {
                rpcMethodInfo = null;
                return false;
            }
            paramIndex++;
        }
        rpcMethodInfo = new RpcMethodInfo(method, rpcAttr.rpcType, rpcArgTypes.ToArray());
        return true;
    }
}