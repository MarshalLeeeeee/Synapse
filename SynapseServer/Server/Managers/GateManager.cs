using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Xml.Linq;

/// <summary>
/// Proxy represents a tcp connection between server and client
/// <para> It handles msg receiving asynchronously </para>
/// </summary>
public class Proxy : ProxyCommon
{
    /// <summary>
    /// exclusive id of owner of this proxy
    /// </summary>
    public string owner_id = "";

    /// <summary>
    /// lastest time stamp of receiving heartbeat
    /// </summary>
    public long lastHeartbeatTime = 0;

    public Proxy(TcpClient client_) : base(client_) { }

    protected override void OnStart()
    {
        lastHeartbeatTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

/// <summary>
/// GateManager is responsible for communication between server and client
/// </summary>
[RegisterManager]
public class GateManager : GateManagerCommon
{

    /// <summary>
    /// new tcp connection listener
    /// </summary>
    private TcpListener? listener;

    /// <summary>
    /// task for tcp listener to work
    /// </summary>
    private Task listenerTask = Task.CompletedTask;

    /// <summary>
    /// cancellation token source for tcp listener task
    /// </summary>
    private CancellationTokenSource listenerCts = new CancellationTokenSource();

    /// <summary>
    /// managed proxies, shared in threads
    /// </summary>
    private ConcurrentDictionary<string, Proxy> proxies = new ConcurrentDictionary<string, Proxy>();

    /// <summary>
    /// Check queue for managed proxies, shared in threads
    /// </summary>
    private ConcurrentQueue<string> checkProxyQueue = new ConcurrentQueue<string>();

    /// <summary>
    /// flag of lifecycle of GateManager, shared in threads
    /// <para> when isActive is false, no more proxies can be added </para>
    /// <para> when isActive is false, update function will do nothing </para>
    /// </summary>
    private volatile bool isActive = false;

    /// <summary>
    /// latest time stamp of check proxies
    /// </summary>
    private long lastCheckTime = 0;

    private ConcurrentQueue<(string proxyId, Msg msg)> msgInbox = new ConcurrentQueue<(string proxyId, Msg msg)>();
    private ConcurrentQueue<(string proxyId, Msg msg)> msgOutbox = new ConcurrentQueue<(string proxyId, Msg msg)>();

    /// <summary>
    /// Start to accept incoming connections
    /// </summary>
    protected override void OnStart()
    {
        StartListenerTask();
        isActive = true;
        Log.Info("GateManager starts...");
    }

    /// <summary>
    /// Update function called in main thread
    /// <para> Hangle queue msg (inbox and outbox) </para>
    /// <para> Check validness of proxies </para>
    /// </summary>
    /// <param name="dt"></param>
    protected override void DoUpdate(float dt)
    {
        if (!isActive) return;
        ConsumeMsgInbox();
        ConsumeMsgOutbox();
        CheckProxies();
    }

    /// <summary>
    /// Remove all proxies
    /// <para> Stop to accept incoming connections </para>
    /// </summary>
    protected override void OnDestroy()
    {
        if (!isActive) return;
        Log.Info("GateManager stops...");
        isActive = false;
        RemoveAllProxies();
        StopListenerTask();
    }

    #region REGION_LISTENER

    /// <summary>
    /// Start listenere task from main thread
    /// </summary>
    private void StartListenerTask()
    {
        if (!listenerTask.IsCompleted)
        {
            Log.Error("[GateManager][StartListenerTask] Listener task is already running...");
            return;
        }
        listenerTask = Task.Run(() => ListenerWorker(listenerCts.Token));
    }

    /// <summary>
    /// Worker function for tcp listener task
    /// <para> Running in an off thread </para>
    /// <para> Listens for and accepts incoming tcp connections asynchronously </para>
    /// </summary>
    /// <param name="ct"> cancellation token </param>
    private async Task ListenerWorker(CancellationToken ct)
    {
        try
        {
            // create and start tcp listener
            listener = new TcpListener(IPAddress.Any, Const.Port);
            listener?.Start();
            Log.Info("[GateManager][ListenerWorker] Tcp Listener started...");
            while (!ct.IsCancellationRequested)
            {
                if (!isActive)
                {
                    Log.Error("[GateManager][ListenerWorker] Tcp Listener is stopping: GateManager is not active");
                    break;
                }

                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    _ = Task.Run(() => HandleNewConnectionAsync(client));
                }
                catch (ObjectDisposedException) when (ct.IsCancellationRequested)
                {
                    Log.Error("[GateManager][ListenerWorker] Tcp Listener has been disposed...");
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.OperationAborted)
                {
                    Log.Error("[GateManager][ListenerWorker] Tcp Listener has been aborted...");
                    break;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    Log.Error("[GateManager][ListenerWorker] Tcp Listener operation has been cancelled...");
                    break;
                }
                catch (Exception ex)
                {
                    Log.Error($"[GateManager][ListenerWorker] Tcp Listener accepts connection failed: {ex}...");
                    await Task.Delay(1000, ct).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            // stop and recycle tcp listener
            OnDestroy();
            Log.Info("[GateManager][ListenerWorker] Tcp Listener stopped...");
        }
    }

    /// <summary>
    /// Handle a new accepted tcp connection
    /// <para> Running in an off thread </para>
    /// </summary>
    /// <param name="client"></param>
    private void HandleNewConnectionAsync(TcpClient client)
    {
        if (!isActive)
        {
            Log.Error("[GateManager][HandleNewConnectionAsync] New connection is rejected: GateManager is not active");
            client.Close();
            return;
        }

        Proxy proxy = new Proxy(client);
        AddProxy(proxy);
        Log.Info("[GateManager][HandleNewConnectionAsync] New connection accepted...");
    }

    private void StopListenerTask()
    {
        if (listenerTask.IsCompleted)
        {
            Log.Error("[GateManager][StopListenerTask] Listener task is not running...");
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
        catch (Exception ex)
        {
            Log.Error($"[GateManager][StopListenerTask] Stop listener task failed: {ex}...");
        }
        finally
        {
            listener?.Stop();
            listener = null;
            listenerCts.Dispose();
            listenerCts = new CancellationTokenSource();
            listenerTask = Task.CompletedTask;
            Log.Info("[GateManager][StopListenerTask] Listener task stopped...");
        }
    }

    #endregion

    #region REGION_PROXY

    /// <summary>
    /// Add a new proxy to be managed and start its functionality
    /// </summary>
    /// <param name="proxy"> Proxy </param>
    private void AddProxy(Proxy proxy)
    {
        string proxyId = proxy.proxyId;
        if (!isActive)
        {
            proxy.Destroy();
            Log.Error($"[GateManager][AddProxy] Proxy {proxyId} is rejected: GateManager is not active");
            return;
        }

        if (proxies.TryAdd(proxyId, proxy))
        {
            proxy.Start(
                OnReceiveMsg,
                RemoveProxy
            );
            checkProxyQueue.Enqueue(proxyId);
            Log.Info($"[GateManager][AddProxy] Proxy [{proxyId}] is added with connection state [{proxy.IsConnected()}]");
        }
        else
        {
            Log.Error($"[GateManager][AddProxy] Proxy {proxyId} fails to be added: already managed");
        }
    }

    /// <summary>
    /// Remove a proxy and stop its functionality
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    private void RemoveProxy(string proxyId)
    {
        if (proxies.TryRemove(proxyId, out Proxy? proxy))
        {
            EventManager? eventManager = Game.Instance.GetManager<EventManager>();
            if (eventManager != null)
            {
                eventManager.TriggerGlobalEvent("OnRemoveProxy", proxyId);
            }
            proxy?.Destroy();
            Log.Info($"[GateManager][RemoveProxy] Proxy [{proxyId}] is removed with connection state [{proxy.IsConnected()}]");
        }
        else
        {
            Log.Error($"[GateManager][RemoveProxy] Proxy {proxyId} fails to be removed: not managed");
        }
    }

    /// <summary>
    /// Remove all proxies, used when GateManager is being destroyed
    /// </summary>
    private void RemoveAllProxies()
    {
        List<string> proxy_ids = new List<string>(proxies.Keys);
        foreach (string proxyId in proxy_ids)
        {
            RemoveProxy(proxyId);
        }
    }

    public Proxy? GetProxy(string proxyId)
    {
        if (proxies.TryGetValue(proxyId, out Proxy? proxy))
        {
            return proxy;
        }
        return null;
    }

    /// <summary>
    /// Get the list of all proxy ids (thread safe)
    /// </summary>
    /// <returns> copy of list of proxy id </returns>
    public List<string> GetProxyIds()
    {
        return proxies.Keys.ToList();
    }

    #endregion

    #region REGION_MSG

    /// <summary>
    /// Send msg (in any thread)
    /// <para> If connection is not valid, reset connection </para>
    /// </summary>
    /// <param name="proxy"> proxy </param>
    /// <param name="msg"> message </param>
    private void AppendSendMsg(Proxy proxy, Msg msg)
    {
        if (!proxy.IsConnected()) return;
        msgOutbox.Enqueue((proxy.proxyId, msg));
    }

    /// <summary>
    /// Consume and handle msg pending for sending (in main thread)
    /// </summary>
    private void ConsumeMsgOutbox()
    {
        int cnt = 0;
        while (cnt < Const.MsgSendCntPerUpdate && msgOutbox.TryDequeue(out var item))
        {
            string proxyId = item.proxyId;
            Proxy? proxy = GetProxy(proxyId);
            if (proxy == null) continue;
            SendMsg(proxy, item.msg);
            cnt += 1;
        }
    }

    /// <summary>
    /// Send msg to proxy (in main thread)
    /// <para> If connection is not valid, do nothing </para>
    /// </summary>
    /// <param name="proxy"> proxy </param>
    /// <param name="msg"> message </param>
    private void SendMsg(Proxy proxy, Msg msg)
    {
        if (!proxy.IsConnected()) return;
        MsgStreamer.WriteMsgToStream(proxy, msg);
    }

    /// <summary>
    /// On proxy receiving invalid msg
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    /// <param name="msg"> message </param>
    private void OnReceiveMsg(string proxyId, Msg? msg)
    {
        if (msg == null) return;
        msgInbox.Enqueue((proxyId, msg));
    }

    /// <summary>
    /// Consume and hangle msg from queue (in main thread)
    /// </summary>
    private void ConsumeMsgInbox()
    {
        int cnt = 0;
        while (cnt < Const.MsgReceiveCntPerUpdate && msgInbox.TryDequeue(out var item))
        {
            string proxyId = item.proxyId;
            Proxy? proxy = GetProxy(proxyId);
            if (proxy == null) continue;
            InvokeRpc(proxy, item.msg);
            cnt += 1;
        }
    }

    #endregion

    #region REGION_HEARTBEAT

    /// <summary>
    /// Check validness of proxies periodically
    /// </summary>
    private void CheckProxies()
    {
        // customized check interval
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now - lastCheckTime < Const.CheckProxyInterval) return;
        lastCheckTime = now;

        int cnt = 0;
        HashSet<string> checkedProxyId = new HashSet<string>();
        while (cnt < Const.CheckProxyCntPerUpdate && checkProxyQueue.TryDequeue(out string? proxyId))
        {
            if (proxyId == null) break;
            if (checkedProxyId.Contains(proxyId))
            {
                checkProxyQueue.Enqueue(proxyId);
                break;
            }

            cnt += 1;
            checkedProxyId.Add(proxyId);
            Proxy? proxy = GetProxy(proxyId);
            if (proxy == null || !proxy.IsConnected() || now - proxy.lastHeartbeatTime > Const.HeartBeatThreshold)
            {
                RemoveProxy(proxyId);
            }
            else
            {
                checkProxyQueue.Enqueue(proxyId);
            }
        }
    }

    [Rpc(RpcConst.AnyClient)]
    public void PingHeartbeatRemote(Proxy proxy)
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        proxy.lastHeartbeatTime = now;
    }

    #endregion

    #region REGION_RPC

    public bool CallRpc(string proxyId, string methodName, string instanceId, params Node[] args)
    {
        Proxy? proxy = GetProxy(proxyId);
        if (proxy == null) return false;
        AppendSendMsg(proxy, new Msg(methodName, instanceId, args));
        return true;
    }

    private void InvokeRpc(Proxy proxy, Msg msg)
    {
        // get rpc method
        string methodName = msg.methodName;
        RpcMethodInfo? rpcMethodInfo = Reflection.GetRpcMethod(methodName);
        if (rpcMethodInfo == null) return;

        // get method owner and instance
        var (owner, instance) = GetRpcOwnerAndInstance(msg.instanceId);
        if (owner == null || instance == null) return;

        // check rpc type
        //if ((rpcMethodInfo.rpcType & RpcConst.OwnClient) != 0 && owner.id != proxy.proxyId) return;

        // check arg len
        if (!rpcMethodInfo.CheckArgTypes(msg.arg)) return;

        // pack and invoke method
        List<object> methodArgs = new List<object>();
        methodArgs.Add(proxy);
        foreach (Node arg in msg.arg)
        {
            methodArgs.Add(arg);
        }
        rpcMethodInfo.Invoke(instance, methodArgs.ToArray());
    }

    #endregion
}
