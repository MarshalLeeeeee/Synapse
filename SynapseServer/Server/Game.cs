using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Game : GameCommon
{
    public static Game Instance { get; private set; } = null!;

    public Game()
    {
        Instance = this;
        InitManagers();
    }
}
