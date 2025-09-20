using System;
using System.Collections.Generic;

public static class Launcher
{
    /* Launch game process */
    public static void LaunchGame()
    {
        Console.Title = ArgParser.GetConsoleTitle();
#if DEBUG
        Debug.StartGmConsole();
#endif
        Reflection.Init(new IServerReflection());
        using (Game game = new Game())
        {
            game.Start();
        }
#if DEBUG
        Debug.StopGmConsole();
#endif
    }

    /* Launch console program for user gm
     * Using pipeline to tell and execute in Game process
     */
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

    /* Launch test mode    
     */
    public static void LaunchTest()
    {
        int totalCnt = 0;
        int succCnt = 0;
        Reflection.Init(new IServerReflection(), true);
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
                Console.WriteLine($"Server Test case {methodName} passed...");
            }
            catch (ApplicationException ex)
            {
                Console.WriteLine($"Server Test case {methodName} failed... due to {ex}");
            }
            catch
            {
                Console.WriteLine($"Server Test case {methodName} failed... due to unknown exceptions");
            }
        }
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine($"Server Tests results: {succCnt}/{totalCnt}");
        if (totalCnt == succCnt)
        {
            Console.WriteLine("Server Tests passed...");
        }
        else
        {
            Console.WriteLine("Server Tests failed...");
            throw new ApplicationException("Server Tests failed...");
        }
    }
}
