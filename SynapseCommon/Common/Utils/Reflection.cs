using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Reflection
{
    /* manager types */
    private static Dictionary<string, Type> managerTypes = new Dictionary<string, Type>();
    ///* entity types */
    //private static Dictionary<string, Type> entityTypes = new Dictionary<string, Type>();
    ///* component types */
    //private static Dictionary<string, Type> componentTypes = new Dictionary<string, Type>();
    ///* node types */
    //private static Dictionary<int, Type> nodeTypes = new Dictionary<int, Type>();
    ///* rpc methods */
    //private static Dictionary<string, MethodInfo> rpcMethods = new Dictionary<string, MethodInfo>();

    public static void Init()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        foreach (Type t in types)
        {
            string typeName = t.Name;

            // register Manager
            IsManagerAttribute? isManagerAttr = t.GetCustomAttribute<IsManagerAttribute>();
            if (isManagerAttr != null && t.IsSubclassOf(typeof(Manager)))
            {
                managerTypes[typeName] = t;
            }

            //// register Entity
            //IsEntityAttribute? isEntityAttr = t.GetCustomAttribute<IsEntityAttribute>();
            //if (isEntityAttr != null && t.IsSubclassOf(typeof(Entity)))
            //{
            //    entityTypes[typeName] = t;
            //}

            //// register Component
            //IsComponentAttribute? isComponentAttr = t.GetCustomAttribute<IsComponentAttribute>();
            //if (isComponentAttr != null && t.IsSubclassOf(typeof(Component)))
            //{
            //    componentTypes[typeName] = t;
            //}

            //if (t.IsSubclassOf(typeof(Node)))
            //{
            //    // register node type
            //    FieldInfo[] staticFields = t.GetFields(
            //        BindingFlags.Static |
            //        BindingFlags.Public
            //    );
            //    foreach (FieldInfo staticField in staticFields)
            //    {
            //        if (staticField.Name == "staticPropType")
            //        {
            //            object? obj = staticField.GetValue(null);
            //            if (obj != null && obj is int nodeType && nodeType != NodeConst.TypeUndefined)
            //            {
            //                nodeTypes[nodeType] = t;
            //            }
            //        }
            //    }

            //    // register rpc method
            //    int rpcType = Const.RpcType;
            //    MethodInfo[] methods = t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            //    foreach (MethodInfo method in methods)
            //    {
            //        var rpcAttr = method.GetCustomAttribute<RpcAttribute>();
            //        if (rpcAttr == null)
            //        {
            //            continue;
            //        }
            //        if ((rpcAttr.rpcType & rpcType) == 0)
            //        {
            //            continue;
            //        }
            //        rpcMethods[method.Name] = method;
            //    }
            //}
        }
    }

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

    /* Create default component by name */
    //public static Component? CreateComponent(string compName)
    //{
    //    if (componentTypes.TryGetValue(compName, out Type compType))
    //    {
    //        object? obj = Activator.CreateInstance(compType);
    //        if (obj != null && obj is Component comp)
    //        {
    //            return comp;
    //        }
    //    }
    //    return null;
    //}

    //public static Type? GetComponentType(string compName)
    //{
    //    if (componentTypes.TryGetValue(compName, out Type compType))
    //    {
    //        return compType;
    //    }
    //    return null;
    //}

    /* Get Deserialize method given node type */
    //public static MethodInfo? GetDeserializeMethod(int nodeType)
    //{
    //    if (nodeTypes.TryGetValue(nodeType, out Type? type))
    //    {
    //        MethodInfo? method = type?.GetMethod(
    //            "Deserialize",
    //            BindingFlags.Static | BindingFlags.Public
    //        );
    //        return method;
    //    }
    //    return null;
    //}
}