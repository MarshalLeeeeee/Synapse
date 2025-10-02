using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class IServerReflection : IReflection
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
        int paramIndex = 0;
        int paramCount = paramaters.Length;
        List<int> rpcArgTypes = new List<int>();
        while (paramIndex < paramCount)
        {
            ParameterInfo param = paramaters[paramIndex];
            Type paramType = param.ParameterType;
            if (paramIndex == 0)
            {
                if (paramType != typeof(Proxy))
                {
                    rpcMethodInfo = null;
                    return false;
                }
            }
            else
            {
                if (!paramType.IsSubclassOf(typeof(Node)) && paramType != typeof(Node))
                {
                    rpcMethodInfo = null;
                    return false;
                }
                SyncNodeAttribute? syncNodeAttr = paramType.GetCustomAttribute<SyncNodeAttribute>();
                if (syncNodeAttr != null)
                {
                    rpcArgTypes.Add(syncNodeAttr.nodeType);
                }
                else
                {
                    rpcMethodInfo = null;
                    return false;
                }
            }
            paramIndex++;
        }
        rpcMethodInfo = new RpcMethodInfo(method, rpcAttr.rpcType, rpcArgTypes.ToArray());
        return true;
    }
}
