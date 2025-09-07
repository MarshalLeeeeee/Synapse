
public static class ArgParser
{
    public static string launchMode = "";

    public static void Parse(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i].ToLower();
            if (arg == "--launch-mode" && i + 1 < args.Length)
            {
                launchMode = args[i + 1].ToLower();
                i++;
            }
        }
    }
}
