using System.Reflection;

public interface IReflection
{
    public bool GetRpcMethodInfo(MethodInfo method, int rpcType, out RpcMethodInfo? rpcMethodInfo);
}

public class Reflection
{
    private static IReflection reflectionImpl;

    /// <summary>
    /// mapping from node type to static deserialize method info (only when [SyncNode])
    /// </summary>
    private static Dictionary<int, MethodInfo> nodeDeserializeMethod = new Dictionary<int, MethodInfo>();

    /// <summary>
    /// mapping from manager class name to manager class type (only when [RegisterManager])
    /// </summary>
    private static Dictionary<string, Type> managerTypes = new Dictionary<string, Type>();

    /// <summary>
    /// mapping from rpc method name to rpc method info (only when [Rpc])
    /// </summary>
    private static Dictionary<string, RpcMethodInfo> rpcMethods = new Dictionary<string, RpcMethodInfo>();

    /// <summary>
    /// mapping from gm class name to gm method info (only when [RegisterGm])
    /// </summary>
    private static Dictionary<string, GmMethodInfo> gmMethods = new Dictionary<string, GmMethodInfo>();

    /// <summary>
    /// mapping from test case method name to method info (only when [RegisterTest])
    /// </summary>
    private static Dictionary<string, MethodInfo> testMethods = new Dictionary<string, MethodInfo>();

    #region REGION_INIT

    public static void Init(IReflection reflectionImpl_, bool isTestMode = false)
    {
        reflectionImpl = reflectionImpl_;
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type t in types)
        {
            string typeName = t.Name;

            RegisterNodeDeserializeMethod(t);
            RegisterManager(t);
            RegisterRpcMethod(t);
#if DEBUG
            RegisterGm(t);
            RegisterTest(t, isTestMode);
#endif
        }
    }

    private static void RegisterNodeDeserializeMethod(Type t)
    {
        SyncNodeAttribute? syncNodeAttr = t.GetCustomAttribute<SyncNodeAttribute>();
        if (syncNodeAttr != null)
        {
            MethodInfo? method = t?.GetMethod(
                "Deserialize",
                BindingFlags.Static | BindingFlags.Public
            );
            if (method != null)
            {
                nodeDeserializeMethod[syncNodeAttr.nodeType] = method;
            }
        }
    }

    private static void RegisterManager(Type t)
    {
        RegisterManagerAttribute? registerManagerAttr = t.GetCustomAttribute<RegisterManagerAttribute>();
        if (registerManagerAttr != null && t.IsSubclassOf(typeof(Manager)))
        {
            managerTypes[t.Name] = t;
        }
    }

    /// <summary>
    /// Register Rpc Methods
    /// <para> - save rpcType </para>
    /// <para> - save argTypes from method parameters </para>
    /// </summary>
    /// <param name="t"> type of the class </param>
    private static void RegisterRpcMethod(Type t)
    {
        int rpcType = Const.RpcType;
        MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
            if (reflectionImpl.GetRpcMethodInfo(method, rpcType, out RpcMethodInfo? rpcMethodInfo))
            {
                if (rpcMethodInfo != null)
                {
                    rpcMethods[$"{t.Name}.{method.Name}"] = rpcMethodInfo;
                }
            }
        }
    }

    /// <summary>
    /// Register Gm Methods
    /// <para> - save argTypes </para>
    /// </summary>
    /// <param name="t"> type of the class </param>
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

    /// <summary>
    /// Register all methods under Test class
    /// <para> methods under Test class are only valid as public static void() </para>
    /// </summary>
    /// <param name="t"> type of the class </param>
    /// <param name="isTestMode"> if current launch mode is test mode </param>
    private static void RegisterTest(Type t, bool isTestMode)
    {
        if (!isTestMode) return;

        RegisterTestAttribute? registerTestAttribute = t.GetCustomAttribute<RegisterTestAttribute>();
        if (registerTestAttribute != null)
        {
            MethodInfo[] methods = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                testMethods[$"{t.Name}.{method.Name}"] = method;
            }
        }
    }

    #endregion

    /// <summary>
    /// Create manager by name
    /// </summary>
    /// <param name="mgrName"> target manager class name </param>
    /// <returns> Manager instance or null </returns>
    public static Manager? CreateManager(string mgrName)
    {
        if (managerTypes.TryGetValue(mgrName, out Type? mgrType))
        {
            if (mgrType != null)
            {
                object? obj = Activator.CreateInstance(mgrType);
                if (obj != null && obj is Manager manager)
                {
                    return manager;
                }
            }
            return null;
        }
        return null;
    }

    /// <summary>
    /// Get all registered manager names
    /// </summary>
    /// <returns> list of name of all registered managers </returns>
    public static List<string> GetAllManagerNames()
    {
        return [.. managerTypes.Keys];
    }

    /// <summary>
    /// Check if the node type is syncable
    /// </summary>
    /// <param name="nodeType"> node type </param>
    /// <returns> If the node type is registered as syncable </returns>
    public static bool IsSyncNode(int nodeType)
    {
        MethodInfo? method = GetDeserializeMethod(nodeType);
        return method != null;
    }

    /// <summary>
    /// Get Deserialize method given node type
    /// </summary>
    /// <param name="nodeType"></param>
    /// <returns> deserialize method or null </returns>
    public static MethodInfo? GetDeserializeMethod(int nodeType)
    {
        if (nodeDeserializeMethod.TryGetValue(nodeType, out MethodInfo? method))
        {
            if (method != null)
            {
                return method;
            }
        }
        return null;
    }

    /// <summary>
    /// Get RpcMethodInfo
    /// </summary>
    /// <param name="methodName"> name of the rpc method </param>
    /// <returns> corresponding rpc method info or null </returns>
    public static RpcMethodInfo? GetRpcMethod(string methodName)
    {
        if (rpcMethods.TryGetValue(methodName, out RpcMethodInfo? method))
        {
            return method;
        }
        return null;
    }

    /// <summary>
    /// Get GmMethodInfo
    /// </summary>
    /// <param name="methodName"> name of the gm method </param>
    /// <returns> corresponding gm method info or null </returns>
    public static GmMethodInfo? GetGmMethod(string methodName)
    {
        if (gmMethods.TryGetValue(methodName, out GmMethodInfo? method))
        {
            return method;
        }
        return null;
    }

    /// <summary>
    /// Get all registered test case methods
    /// </summary>
    /// <returns> Iterator of test case name with test case method info </returns>
    public static IEnumerable<KeyValuePair<string, MethodInfo>> IterTestMethods()
    {
        foreach (var kvp in testMethods)
        {
            yield return kvp;
        }
    }
}