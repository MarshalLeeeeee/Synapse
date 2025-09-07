
public static class ArgParser
{
    /* launch mode */
    public static string launchMode = "";
    /* title of the console window
     * if empty, use Const.Title
     */
    public static string title = "";

    public static void Parse(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i].ToLower();
            if (arg == "--launch-mode" && i + 1 < args.Length)
            {
                launchMode = args[i + 1];
                i++;
            }
            else if (arg == "--title" && i + 1 < args.Length)
            {
                title = args[i + 1];
                i++;
            }
        }
    }

    public static string GetConsoleTitle()
    {
        if (!string.IsNullOrEmpty(title)) return title;
        else
        {
            int processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            return Const.Title + $"-{processId}";
        }

    }
}
