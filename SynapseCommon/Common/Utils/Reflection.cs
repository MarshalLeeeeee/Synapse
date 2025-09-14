using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public interface IReflection
{
    public bool GetRpcMethodInfo(MethodInfo method, int rpcType, out RpcMethodInfo? rpcMethodInfo);
}

public class Reflection
{
    private static IReflection reflectionImpl;

    /* node types */
    private static Dictionary<int, Type> nodeTypes = new Dictionary<int, Type>();
    /* manager types */
    private static Dictionary<string, Type> managerTypes = new Dictionary<string, Type>();
    /* rpc methods */
    private static Dictionary<string, RpcMethodInfo> rpcMethods = new Dictionary<string, RpcMethodInfo>();
    /* gm methods */
    private static Dictionary<string, GmMethodInfo> gmMethods = new Dictionary<string, GmMethodInfo>();

    #region REGION_INIT

    public static void Init(IReflection reflectionImpl_)
    {
        reflectionImpl = reflectionImpl_;
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type t in types)
        {
            string typeName = t.Name;

            RegisterNode(t);
            RegisterManager(t);
            RegisterRpcMethod(t);
#if DEBUG
            RegisterGm(t);
#endif
        }
    }

    private static void RegisterNode(Type t)
    {
        if (t.IsSubclassOf(typeof(Node)))
        {
            object? value = t.GetField(
                "staticNodeType",
                BindingFlags.Static | BindingFlags.Public
            )?.GetValue(null);
            if (value != null && value is int nodeType && nodeType != NodeConst.TypeUndefined)
            {
                nodeTypes[nodeType] = t;
            }
        }
    }

    private static void RegisterManager(Type t)
    {
        RegisterManagerAttribute? RegisterManagerAttr = t.GetCustomAttribute<RegisterManagerAttribute>();
        if (RegisterManagerAttr != null && t.IsSubclassOf(typeof(Manager)))
        {
            managerTypes[t.Name] = t;
        }
    }

    /* Register Rpc Methods
     * - save rpcType
     * - save argTypes from method parameters
     */
    private static void RegisterRpcMethod(Type t)
    {
        if (t.IsSubclassOf(typeof(Node)))
        {
            int rpcType = Const.RpcType;
            MethodInfo[] methods = t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                if (reflectionImpl.GetRpcMethodInfo(method, rpcType, out RpcMethodInfo? rpcMethodInfo))
                {
                    if (rpcMethodInfo != null)
                    {
                        rpcMethods[method.Name] = rpcMethodInfo;
                    }
                }
            }
        }
    }

    /* Register Gm Methods
     * - save argTypes
     */
    private static void RegisterGm(Type t)
    {
        RegisterGmAttribute? registerGmAttribute = t.GetCustomAttribute<RegisterGmAttribute>();
        if (registerGmAttribute != null)
        {
            MethodInfo? method = t.GetMethod(
                "Execute",
                BindingFlags.Static | BindingFlags.Public
            );
            if (method != null)
            {
                ParameterInfo[] paramaters = method.GetParameters();
                List<int> gmArgTypes = new List<int>();
                bool validGm = true;
                foreach (ParameterInfo param in paramaters)
                {
                    int gmArgType = Debug.GetGmArgType(param.ParameterType);
                    if (gmArgType == GmConst.TypeUndefined)
                    {
                        validGm = false;
                        Log.Error($"Gm method {method.Name} has invalid parameter type {param.ParameterType.Name}");
                        break;
                    }
                    else
                    {
                        gmArgTypes.Add(gmArgType);
                    }
                }
                if (validGm)
                {
                    gmMethods[t.Name] = new GmMethodInfo(method, gmArgTypes.ToArray());
                }
            }
        }
    }

    #endregion

    /* Create manager by name */
    public static Manager? CreateManager(string mgrName)
    {
        if (managerTypes.TryGetValue(mgrName, out Type mgrType))
        {
            object? obj = Activator.CreateInstance(mgrType);
            if (obj != null && obj is Manager manager)
            {
                return manager;
            }
        }
        return null;
    }

    /* Get all registered manager names */
    public static List<string> GetAllManagerNames()
    {
        return [.. managerTypes.Keys];
    }

    /* Get Deserialize method given node type */
    public static MethodInfo? GetDeserializeMethod(int nodeType)
    {
        if (nodeTypes.TryGetValue(nodeType, out Type? type))
        {
            MethodInfo? method = type?.GetMethod(
                "Deserialize",
                BindingFlags.Static | BindingFlags.Public
            );
            return method;
        }
        return null;
    }

    /* Get RpcMethodInfo */
    public static RpcMethodInfo? GetRpcMethod(string methodName)
    {
        if (rpcMethods.TryGetValue(methodName, out RpcMethodInfo? method))
        {
            return method;
        }
        return null;
    }

    public static GmMethodInfo? GetGmMethod(string methodName)
    {
        if (gmMethods.TryGetValue(methodName, out GmMethodInfo? method))
        {
            return method;
        }
        return null;
    }
}