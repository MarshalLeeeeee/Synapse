using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

/*
 * Proxy represents a tcp connection between server and client
 * It handles msg receiving asynchronously
 */
public class ProxyCommon
{
    /* exclusive id of proxy */
    public string proxyId = "";

    /* tcp connection */
    protected TcpClient? client;
    /* task for message listener to work */
    protected Task msgListenerTask = Task.CompletedTask;
    /* cancellation token source for message listener task */
    protected CancellationTokenSource msgListenerCts = new CancellationTokenSource();

    /* network stream in tcp connection */
    public NetworkStream? stream => client?.GetStream();

    /* flag of activeness of proxy, shared in threads */
    protected volatile bool isActive = false;

    protected ProxyCommon(TcpClient client_)
    {
        proxyId = Guid.NewGuid().ToString();
        client = client_;
    }

    public void Start(Action<string, Msg?> onReceiveMsgCallback, Action<string> onDisconnectCallback)
    {
        StartListenerTask(onReceiveMsgCallback, onDisconnectCallback);
        OnStart();
        isActive = true;
    }

    protected virtual void OnStart() { }

    public void Destroy()
    {
        if (!isActive) return;

        isActive = false;
        OnDestroy();
        StopListenerTask();
    }

    protected virtual void OnDestroy() { }

    /* Check if tcp client is connected */
    public virtual bool IsConnected()
    {
        if (!isActive) return false;
        if (client?.Client == null) return false;
        try
        {
            return !(client.Client.Poll(0, SelectMode.SelectRead) && client.Client.Available == 0);
        }
        catch
        {
            return false;
        }
    }

    #region REGION_MSG_LISTENER

    /*
     * Start msg listener task
     */
    private void StartListenerTask(Action<string, Msg?> onReceiveMsgCallback, Action<string> onDisconnectCallback)
    {
        if (!msgListenerTask.IsCompleted)
        {
            Log.Error("[Proxy][StartListenerTask] Message listener task is already running...");
            return;
        }
        msgListenerTask = Task.Run(() => MsgListenerWorker(onReceiveMsgCallback, onDisconnectCallback, msgListenerCts.Token));
    }

    /*
     * Worker function of msg listener task, dealing with incoming msgs from tcp client
     * Running in an off thread
     */
    private async Task MsgListenerWorker(Action<string, Msg?> onReceiveMsgCallback, Action<string> onDisconnectCallback, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    if (!IsConnected())
                    {
                        Log.Error($"[Proxy][MsgListenerWorker] Proxy [{proxyId}] lost connection in msg listener");
                        break;
                    }

                    var res = await MsgStreamer.ReadMsgFromStreamAsync(stream, ct).ConfigureAwait(false);
                    if (res.succ)
                    {
                        onReceiveMsgCallback(proxyId, res.msg);
                    }
                    else
                    {
                        Log.Error($"[Proxy][MsgListenerWorker] Proxy [{proxyId}] Invalid message");
                        await Task.Delay(1000, ct).ConfigureAwait(false);
                    }
                }
                catch (ObjectDisposedException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    break;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error($"[Proxy][MsgListenerWorker] Proxy [{proxyId}] Invalid message with error: {ex}");
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            onDisconnectCallback(proxyId);
            Destroy();
            Log.Info($"[Proxy][MsgListenerWorker] Proxy [{proxyId}] disconnected and cleaned up...");
        }
    }

    /*
     * Stop msg listener task
     */
    private void StopListenerTask()
    {
        if (msgListenerTask.IsCompleted)
        {
            Log.Error("[Proxy][StopListenerTask] Message listener task is not running...");
            return;
        }

        msgListenerCts.Cancel();
        try
        {
            msgListenerTask.Wait(1000);
        }
        catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerException is OperationCanceledException)
        {
            // Ignore OperationCanceledException
        }
        catch (Exception ex)
        {
            Log.Error($"[Proxy][StopListenerTask] Stop message listener task failed: {ex}...");
        }
        finally
        {
            client?.Close();
            client = null;
            msgListenerCts.Dispose();
            msgListenerCts = new CancellationTokenSource();
            msgListenerTask = Task.CompletedTask;
            Log.Info("[Proxy][StopListenerTask] Message listener task stopped...");
        }
    }

    #endregion
}

/*
 * GateManager is responsible for communication between server and client
 */
public class GateManagerCommon : Manager
{

}
