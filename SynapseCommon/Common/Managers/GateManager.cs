using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Proxy represents a tcp connection between server and client
/// <para> It handles msg receiving asynchronously </para>
/// </summary>
public class ProxyCommon
{
    /// <summary>
    /// exclusive id of proxy
    /// </summary>
    public string proxyId = "";

    /// <summary>
    /// tcp connection
    /// </summary>
    protected TcpClient? client;

    /// <summary>
    /// task for message listener to work
    /// </summary>
    protected Task msgListenerTask = Task.CompletedTask;

    /// <summary>
    /// cancellation token source for message listener task
    /// </summary>
    protected CancellationTokenSource msgListenerCts = new CancellationTokenSource();

    /// <summary>
    /// network stream in tcp connection
    /// </summary>
    public NetworkStream? stream => client?.GetStream();

    /// <summary>
    /// flag of activeness of proxy, shared in threads
    /// </summary>
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

    /// <summary>
    /// Check if tcp client is connected
    /// </summary>
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

    /// <summary>
    /// Start msg listener task
    /// </summary>
    /// <param name="onReceiveMsgCallback"> callback when new message is received </param>
    /// <param name="onDisconnectCallback"> callback when proxy is disconnected </param>
    private void StartListenerTask(Action<string, Msg?> onReceiveMsgCallback, Action<string> onDisconnectCallback)
    {
        if (!msgListenerTask.IsCompleted)
        {
            Log.Error("[Proxy][StartListenerTask] Message listener task is already running...");
            return;
        }
        msgListenerTask = Task.Run(() => MsgListenerWorker(onReceiveMsgCallback, onDisconnectCallback, msgListenerCts.Token));
    }

    /// <summary>
    /// Worker function of msg listener task, dealing with incoming msgs from tcp client
    /// <para> Running in off thread </para>
    /// </summary>
    /// <param name="onReceiveMsgCallback"> callback when new message is received </param>
    /// <param name="onDisconnectCallback"> callback when proxy is disconnected </param>
    /// <param name="ct"> cancellation token </param>
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

    /// <summary>
    /// Stop msg listener task
    /// </summary>
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

/// <summary>
/// GateManager is responsible for communication between server and client
/// </summary>
public class GateManagerCommon : Manager
{
    #region REGION_RPC

    protected static (object? owner, object? instance) GetRpcOwnerAndInstance(string instanceId)
    {
        string[] seg = instanceId.Split('.');
        if (seg.Length == 0) return (null, null);

        string ownerId = seg[0];
        if (ownerId.StartsWith("Mgr-"))
        {
            Manager? mgr = Game.Instance.GetManager(ownerId);
            return (mgr, mgr);
        }
        else if (ownerId.StartsWith("Ett-"))
        {
            PlayerEntity? player = Game.Instance.GetManager<EntityManager>()?.GetPlayerEntity(ownerId);
            return (player, player?.GetChildWithPath(seg[1..]));
        }
        else return (null, null);
    }

    #endregion
}
