using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.IO.Pipes;

/// <summary>
/// Use pipeline to receive and execute gm in Game thread
/// </summary>
public class DebugManagerCommon : Manager
{
    /// <summary>
    /// backgroud task to listen gm command
    /// </summary>
    protected Task listenerTask = Task.CompletedTask;

    /// <summary>
    /// cancellation token source for gm listener task
    /// </summary>
    protected CancellationTokenSource listenerCts = new CancellationTokenSource();

    /// <summary>
    /// flag of activeness of gm listener, shared in threads
    /// </summary>
    protected volatile bool isActive = false;

    /// <summary>
    /// queue for gm command to be executed, shared in threads
    /// </summary>
    protected ConcurrentQueue<string> gmQueue = new ConcurrentQueue<string>();

    protected override void OnStart()
    {
        Log.Info("DebugManager started...");
        isActive = true;
        StartListenerTask();
    }

    protected override void DoUpdate(float dt)
    {
        ConsumeGmQuueue();
    }

    protected override void OnDestroy()
    {
        if (!isActive) return;
        isActive = false;
        StopListenerTask();
        Log.Info("DebugManager stopped...");
    }

    #region REGION_GM_LISTENER

    private void StartListenerTask()
    {
        if (!listenerTask.IsCompleted)
        {
            Log.Error("[DebugManager][StartLisenerTasj] Gm listener task is already running...");
            return;
        }
        listenerTask = Task.Run(() => ListenerWorker(listenerCts.Token));
    }

    private async Task ListenerWorker(CancellationToken ct)
    {
        try
        {
            string pipelineName = Debug.GetGmPipelineName();
            while (!ct.IsCancellationRequested)
            {
                using (NamedPipeServerStream server = new NamedPipeServerStream(pipelineName))
                {
                    await server.WaitForConnectionAsync();
                    using (StreamReader reader = new StreamReader(server))
                    {
                        string newText = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(newText))
                        {
                            gmQueue.Enqueue(newText);
                        }
                    }
                }
            }
        }
        finally
        {
            OnDestroy();
            Log.Info("[DebugManager][ListenerWorker] Gm listener task stopped...");
        }
    }

    private void StopListenerTask()
    {
        if (listenerTask.IsCompleted)
        {
            Log.Error("[DebugManager][StopListenerTask] Gm listener task is not running...");
            return;
        }

        listenerCts.Cancel();
        try
        {
            listenerTask.Wait(1000);
        }
        catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerException is OperationCanceledException)
        {
            // Ignore OperationCanceledException
        }
        finally
        {
            listenerCts.Dispose();
            listenerCts = new CancellationTokenSource();
            listenerTask = Task.CompletedTask;
        }
    }

    #endregion

    #region REGION_GM_EXECUTION

    /// <summary>
    /// execute all queued gm commands
    /// </summary>
    private void ConsumeGmQuueue()
    {
        while (gmQueue.TryDequeue(out string? gm))
        {
            if (gm == null) continue;
            ExecuteGm(gm);
        }
    }

    /// <summary>
    /// execute gm
    /// </summary>
    /// <param name="gm"> raw string of the input gm command </param>
    private void ExecuteGm(string gm)
    {
        Debug.ExecuteGm(gm);
    }

    #endregion
}