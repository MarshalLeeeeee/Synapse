using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;

public static class Debug
{
    private static Process? gmConsoleProcess = null;

    /* Launch gm console process used for communicationg between user gm input and game thread
     * Supposed to be called by game process
     */
    public static void StartGmConsole()
    {
        gmConsoleProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule?.FileName,
                Arguments = "--launch-mode gm",
                UseShellExecute = true,
                CreateNoWindow = false
            }
        };
        gmConsoleProcess.Start();
    }

    /* Kill gm console process
     * Supposed to be called by game process
     */
    public static void StopGmConsole()
    {
        if (gmConsoleProcess != null && !gmConsoleProcess.HasExited)
        {
            gmConsoleProcess.Kill();
            gmConsoleProcess.WaitForExit();
            gmConsoleProcess.Dispose();
            gmConsoleProcess = null;
        }
    }

    /* Write gm to game process from gm precoess through pipeline name
     * Supposed to be called by gm process
     */
    public static void ExecuteGmPipeline(string gm)
    {
        string pipelineName = GetGmPipelineName();
        using (NamedPipeClientStream client = new NamedPipeClientStream(pipelineName))
        {
            client.Connect();
            using (StreamWriter writer = new StreamWriter(client))
            {
                writer.Write(gm);
                writer.Flush();
                Console.WriteLine($"Execute: {gm}");
            }
        }
    }

    public static string GetGmPipelineName()
    {
        return Const.Title + " - GmPipeline";
    }
}