using System;
using System.Collections.Generic;

public static class Launcher
{
    public static void Launch()
    {
        Reflection.Init();
        using (Game game = new Game())
        {
            game.Start();
        }
    }
}
