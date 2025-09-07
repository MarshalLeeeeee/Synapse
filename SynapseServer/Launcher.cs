using System;
using System.Collections.Generic;

public static class Launcher
{
    /* Launch game process */
    public static void LaunchGame()
    {
        Console.Title = ArgParser.GetConsoleTitle();
        Debug.StartGmConsole();
        Reflection.Init();
        using (Game game = new Game())
        {
            game.Start();
        }
        Debug.StopGmConsole();
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
}
