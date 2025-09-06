using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

/*
 * Proxy represents a tcp connection between server and client
 * It handles msg receiving asynchronously
 */
public class Proxy : ProxyCommon
{
    /* exclusive id of owner of this proxy */
    public string owner_id = "";
    /* lastest time stamp of receiving heartbeat */
    public long lastHeartbeatTime = 0;

    public Proxy(TcpClient client_) : base(client_) { }

    protected override void OnStart()
    {
        lastHeartbeatTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}

/*
 * GateManager is responsible for communication between server and client
 */
[IsManager]
public class GateManager : GateManagerCommon
{

    /* new tcp connection listener */
    private TcpListener? listener;
    /* task for tcp listener to work */
    private Task listenerTask = Task.CompletedTask;
    /* cancellation token source for tcp listener task */
    private CancellationTokenSource listenerCts = new CancellationTokenSource();

    /* managed proxies, shared in threads */
    private ConcurrentDictionary<string, Proxy> proxies = new ConcurrentDictionary<string, Proxy>();
    /* Check queue for managed proxies, shared in threads */
    private ConcurrentQueue<string> checkProxyQueue = new ConcurrentQueue<string>();
    /* flag of lifecycle of GateManager, shared in threads 
     * when isActive is false, no more proxies can be added
     # when isActive is false, update function will do nothing
     */
    private volatile bool isActive = false;
    /* latest time stamp of check proxies */
    private long lastCheckTime = 0;

    private ConcurrentQueue<(string proxyId, Msg msg)> msgInbox = new ConcurrentQueue<(string proxyId, Msg msg)>(); // thread shared
    private ConcurrentQueue<(string proxyId, Msg msg)> msgOutbox = new ConcurrentQueue<(string proxyId, Msg msg)>(); // thread shared

    /*
     * Start to accept incoming connections
     */
    protected override void OnStart()
    {
        StartListenerTask();
        isActive = true;
        Log.Info("GateManager starts...");
    }

    /*
     * Update function called in main thread
     ** Hangle queue msg (inbox and outbox)
     ** Check validness of proxies
     */
    protected override void DoUpdate(float dt)
    {
        if (!isActive) return;
        ConsumeMsgInbox();
        ConsumeMsgOutbox();
        CheckProxies();
    }

    /*
     * Remove all proxies
     * Stop to accept incoming connections
     */
    protected override void OnDestroy()
    {
        if (!isActive) return;
        Log.Info("GateManager stops...");
        isActive = false;
        RemoveAllProxies();
        StopListenerTask();
    }

    #region REGION_LISTENER

    /*
     * Start listenere task from main thread
     */
    private void StartListenerTask()
    {
        if (!listenerTask.IsCompleted)
        {
            Log.Error("[GateManager][StartListenerTask] Listener task is already running...");
            return;
        }
        listenerTask = Task.Run(() => ListenerWorker(listenerCts.Token));
    }

    /*
     * Worker function for tcp listener task
     * Running in an off thread
     * Listens for and accepts incoming tcp connections asynchronously
     */
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

    /*
     * Handle a new accepted tcp connection
     * Running in an off thread
     */
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

    /*
     * Add a new proxy to be managed and start its functionality
     */
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

    /*
     * Remove a proxy and stop its functionality
     */
    private void RemoveProxy(string proxyId)
    {
        if (proxies.TryRemove(proxyId, out Proxy? proxy))
        {
            proxy?.Destroy();
            Log.Info($"[GateManager][RemoveProxy] Proxy [{proxyId}] is removed with connection state [{proxy.IsConnected()}]");
        }
        else
        {
            Log.Error($"[GateManager][RemoveProxy] Proxy {proxyId} fails to be removed: not managed");
        }
    }

    /*
     * Remove all proxies, used when GateManager is being destroyed
     */
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

    #endregion

    #region REGION_MSG

    /*
     * Send msg (in any thread)
     * If connection is not valid, reset connection
     */
    public void AppendSendMsg(Proxy proxy, Msg msg)
    {
        if (!proxy.IsConnected()) return;
        msgOutbox.Enqueue((proxy.proxyId, msg));
    }

    /*
     * Consume and handle msg pending for sending (in main thread)
     */
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

    /*
     * Send msg to proxy (in main thread)
     * If connection is not valid, do nothing
     */
    private void SendMsg(Proxy proxy, Msg msg)
    {
        if (!proxy.IsConnected()) return;
        MsgStreamer.WriteMsgToStream(proxy.stream, msg);
    }

    /* On proxy receiving invalid msg */
    private void OnReceiveMsg(string proxyId, Msg? msg)
    {
        if (msg == null) return;
        msgInbox.Enqueue((proxyId, msg));
    }
    
    /*
     * Consume and hangle msg from queue (in main thread)
     */
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

    /*
     * Check validness of proxies periodically
     */
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
            if (checkedProxyId.Contains(proxyId)) break;

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

    private void InvokeRpc(Proxy proxy, Msg msg)
    {
        // get rpc method
        string methodName = msg.methodName;
        MethodInfo? rpcMethod = Reflection.GetRpcMethod(methodName);
        if (rpcMethod == null) return;

        // get method instance
        // TODO
        object? instance = null;
        if (instance == null) return;

        // check rpc type
        var rpcAttr = rpcMethod.GetCustomAttribute<RpcAttribute>();
        if (rpcAttr == null) return;

        // check arg len
        ListNode args = msg.arg;
        int[] rpcArgs = rpcAttr.argTypes;
        int argsCount = args.Count;
        int rpcArgsCount = rpcArgs.Length;
        if (argsCount != rpcArgsCount) return;

        // check arg type
        int i = 0;
        while (i < rpcArgsCount)
        {
            Node arg = args[i];
            if (rpcArgs[i] != NodeConst.TypeUndefined && arg.nodeType != rpcArgs[i]) return;
            i += 1;
        }

        // pack and invoke method
        List<object> methodArgs = new List<object>();
        foreach (Node arg in args)
        {
            methodArgs.Add(arg);
        }
        methodArgs.Add(proxy);
        rpcMethod.Invoke(instance, methodArgs.ToArray());
    }

    #endregion
}
