using System;
using System.Collections.Generic;
using System.Reflection;

public static class Launcher
{
    /// <summary>
    /// Launch game process
    /// </summary>
    public static void LaunchGame()
    {
        Console.Title = ArgParser.GetConsoleTitle();
#if DEBUG
        Debug.StartGmConsole();
#endif
        Reflection.Init(new IClientReflection());
        using (Game game = new Game())
        {
            game.Start();
        }
#if DEBUG
        Debug.StopGmConsole();
#endif
    }

    /// <summary>
    /// Launch console program for user gm
    /// <para> Using pipeline to tell and execute in Game process </para>
    /// </summary>
    public static void LaunchGm()
    {
        Console.Title = ArgParser.GetConsoleTitle() + "-Gm";
        while (true)
        {
            Console.Write("Input: ");
            string? input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input))
            {
                Debug.ExecuteGmPipeline(input);
            }
        }
    }

    /// <summary>
    /// Launch test mode   
    /// </summary>
    /// <exception cref="ApplicationException"></exception>
    public static void LaunchTest()
    {
        int totalCnt = 0;
        int succCnt = 0;
        Reflection.Init(new IClientReflection(), true);
        foreach (var kvp in Reflection.IterTestMethods())
        {
            string methodName = kvp.Key;
            MethodInfo method = kvp.Value;
            totalCnt += 1;
            Console.WriteLine("-----------------------------------------");
            try
            {
                method.Invoke(null, []);
                succCnt += 1;
                Console.WriteLine($"Client Test case {methodName} passed...");
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine($"Client Test case {methodName} failed... {ex}");
            }
            catch
            {
                Console.WriteLine($"Client Test case {methodName} failed... unknown exceptions");
            }
        }
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"Client Tests results: {succCnt}/{totalCnt}");
        if (totalCnt == succCnt)
        {
            Console.WriteLine("Client Tests passed...");
        }
        else
        {
            Console.WriteLine("Client Tests failed...");
            throw new ApplicationException("Client Tests failed...");
        }
    }
}
