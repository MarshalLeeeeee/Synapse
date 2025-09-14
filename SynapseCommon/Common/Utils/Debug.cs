using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;
using System.Reflection;

public static class GmConst
{
    public const int TypeUndefined = 0;
    public const int TypeInt = 1;
    public const int TypeFloat = 2;
    public const int TypeString = 3;
    public const int TypeBool = 4;
}

public class GmMethodInfo
{
    public MethodInfo methodInfo;
    public int[] argTypes;

    public int argCount { get { return argTypes.Length; } }

    public GmMethodInfo(MethodInfo methodInfo_, int[] argTypes_)
    {
        methodInfo = methodInfo_;
        argTypes = argTypes_;
    }

    public void Invoke(object? instance, object[] methodArgs)
    {
        methodInfo.Invoke(instance, methodArgs);
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class RegisterGmAttribute : Attribute
{
    public int[] argTypes { get; }
    public RegisterGmAttribute(params int[] argTypes_)
    {
        argTypes = argTypes_;
    }
}

public static class Debug
{
    private static Process? gmConsoleProcess = null;

    /* Launch gm console process used for communicationg between user gm input and game thread
     * Supposed to be called by game process
     */
    public static void StartGmConsole()
    {
        string title = ArgParser.GetConsoleTitle();
        gmConsoleProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule?.FileName,
                Arguments = $"--launch-mode gm --title {title}",
                UseShellExecute = true,
                CreateNoWindow = false
            }
        };
        gmConsoleProcess.Start();
    }

    /* Kill gm console process
     * Supposed to be called by game process
     */
    public static void StopGmConsole()
    {
        if (gmConsoleProcess != null && !gmConsoleProcess.HasExited)
        {
            gmConsoleProcess.Kill();
            gmConsoleProcess.WaitForExit();
            gmConsoleProcess.Dispose();
            gmConsoleProcess = null;
        }
    }

    /* Write gm to game process from gm precoess through pipeline name
     * Supposed to be called by gm process
     */
    public static void ExecuteGmPipeline(string gm)
    {
        string pipelineName = GetGmPipelineName();
        using (NamedPipeClientStream client = new NamedPipeClientStream(pipelineName))
        {
            client.Connect();
            using (StreamWriter writer = new StreamWriter(client))
            {
                writer.Write(gm);
                writer.Flush();
                Console.WriteLine($"Execute: {gm}");
            }
        }
    }

    public static string GetGmPipelineName()
    {
        return Const.Title + "-GmPipeline";
    }

    /* Convert supported gm parameter type to gm arg type
     * If not supported, return TypeUndefined
     */
    public static int GetGmArgType(Type t)
    {
        if (t == typeof(int)) return GmConst.TypeInt;
        if (t == typeof(float)) return GmConst.TypeFloat;
        if (t == typeof(string)) return GmConst.TypeString;
        if (t == typeof(bool)) return GmConst.TypeBool;
        return GmConst.TypeUndefined;
    }

    /* Parser string input string to corresponding type */
    public static object? ParseGmArg(string argStr, int argType)
    {
        try
        {
            switch (argType)
            {
                case GmConst.TypeInt:
                    return int.Parse(argStr);
                case GmConst.TypeFloat:
                    return float.Parse(argStr);
                case GmConst.TypeString:
                    return argStr;
                case GmConst.TypeBool:
                    return bool.Parse(argStr);
                default:
                    Log.Debug($"Unsupported gm arg type {argType}");
                    return null;
            }
        }
        catch (Exception)
        {
            Log.Debug($"Failed to parse gm arg {argStr} to type {argType}");
            return null;
        }
    }

    /* Execute gm command in game process
     * - Split gm command by space
     * - Find the corresponding gm method using the first arg
     * - Parse the rest args to corresponding type
     * - Invoke the gm method
     */
    public static void ExecuteGm(string gm)
    {
        string[] parts = gm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        string gmName = parts[0];
        GmMethodInfo? gmMethodInfo = Reflection.GetGmMethod(gmName);
        if (gmMethodInfo == null) return;

        // check arg len
        int argCount = parts.Length - 1;
        if (argCount != gmMethodInfo.argCount) return;

        // parse args
        List<object> methodArgs = new List<object>();
        for (int i = 0; i < argCount; i++)
        {
            string argStr = parts[i + 1];
            int argType = gmMethodInfo.argTypes[i];
            object? arg = ParseGmArg(argStr, argType);
            if (arg == null) return;
            methodArgs.Add(arg);
        }

        // invoke method
        gmMethodInfo.Invoke(null, methodArgs.ToArray());
    }
}