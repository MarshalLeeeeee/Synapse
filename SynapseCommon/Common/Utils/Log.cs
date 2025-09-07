using System;
using System.Collections;
using System.Collections.Generic;

public static class Log
{
    public static void Info(string msg)
    {
        Console.WriteLine($"[INFO] {msg}");
    }

    public static void Error(string msg)
    {
        Console.WriteLine($"[ERROR] {msg}");
    }

    public static void Debug(string msg)
    {
#if DEBUG
        Console.WriteLine($"[DEBUG] {msg}");
#endif
    }
}