
class Program
{
    static void Main(string[] args)
    {
        ArgParser.Parse(args);
        string launchMode = ArgParser.launchMode;
        if (launchMode == "game") Launcher.LaunchGame();
        else if (launchMode == "gm") Launcher.LaunchGm();
        else Console.WriteLine($"Unknown launch mode: [{launchMode}]");
    }
}
