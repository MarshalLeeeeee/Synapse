using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Launcher
{
    /* Launch game process */
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
