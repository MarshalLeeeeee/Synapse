
#if DEBUG

using System.Diagnostics;

public class ArgParseConfig
{
    public string launchMode = "";
    public string title = "";
    public ArgParseConfig(
        string launchMode_,
        string title_
    )
    {
        launchMode = launchMode_;
        title = title_;
    }
}

#endif


public static class ArgParser
{
    /// <summary>
    /// launch mode of the process
    /// </summary>
    public static string launchMode = "";

    /// <summary>
    /// title of the console window, if empty, use Const.Title
    /// </summary>
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

#if DEBUG

    /// <summary>
    /// dump a snapshot
    /// </summary>
    /// <returns> current snapshot of config </returns>
    public static ArgParseConfig Dump()
    {
        return new ArgParseConfig(launchMode, title);
    }

    /// <summary>
    /// Recover config from a snapshot
    /// </summary>
    /// <param name="config"> snapshot of config </param>
    public static void Load(ArgParseConfig config)
    {
        launchMode = config.launchMode;
        title = config.title;
    }

    /// <summary>
    /// clear static config
    /// </summary>
    public static void Clear()
    {
        launchMode = "";
        title = "";
    }
#endif
}

#if DEBUG

[RegisterTest]
public static class TestArgParser
{
    public static void TestParse()
    {
        ArgParseConfig config = ArgParser.Dump();

        ArgParser.Clear();
        ArgParser.Parse([""]);
        Assert.Equal("", ArgParser.launchMode, "ArgParser launchMode is not [empty] with [no args]");
        Assert.Equal("", ArgParser.title, "ArgParser title is not [empty] with [no args]");

        ArgParser.Clear();
        ArgParser.Parse(["--launch-mode", "test", "-title", "title"]);
        Assert.Equal("test", ArgParser.launchMode, "ArgParser launchMode is not [test] with [--launch-mode test -title title]");
        Assert.Equal("", ArgParser.title, "ArgParser title is not [empty] with [--launch-mode test -title title]");

        ArgParser.Clear();
        ArgParser.Parse(["--title", "title2", "--launch-mode", "test2"]);
        Assert.Equal("test2", ArgParser.launchMode, "ArgParser launchMode is not [test2] with [--title title2 --launch-mode test]");
        Assert.Equal("title2", ArgParser.title, "ArgParser title is not [title2] with [--title title2 --launch-mode test]");

        ArgParser.Clear();
        ArgParser.Parse(["--launch-mode", "test3", "--title", "title3", "--launch-mode", "test33"]);
        Assert.Equal("test33", ArgParser.launchMode, "ArgParser launchMode is not [test2] with [--launch-mode test3 --title title3 --launch-mode test33]");
        Assert.Equal("title3", ArgParser.title, "ArgParser title is not [title2] with [--launch-mode test3 --title title3 --launch-mode test33]");

        ArgParser.Load(config);
    }
}

#endif
