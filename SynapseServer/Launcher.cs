using System;
using System.Collections.Generic;

public static class Launcher
{
    public static void Launch()
    {
        using (Game game = new Game())
        {
            game.Start();
        }
        Console.WriteLine("Press enter to exit thread...");
        Console.ReadLine();
    }
}
