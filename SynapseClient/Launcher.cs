using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
