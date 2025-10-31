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
public class Proxy : ProxyCommon
{
    /// <summary>
    /// lastest time stamp of sending heartbeat
    /// </summary>
    public long lastHeartbeatTime = 0;

    public Proxy(TcpClient client_) : base(client_) { }
}

/// <summary>
/// GateManager is responsible for communication between server and client
/// </summary>
[RegisterManager]
public class GateManager : GateManagerCommon
{
    private Proxy? proxy;

    /// <summary>
    /// flag to indicate if client is connecting async
    /// <para> Cross thread </para>
    /// </summary>
    private volatile bool isConnecting = false;

    /// <summary>
    /// ping heartbeat time stamp
    /// </summary>
    long lastHeartbeatTime = 0;

    private ConcurrentQueue<Msg> msgInbox = new ConcurrentQueue<Msg>();
    private ConcurrentQueue<Msg> msgOutbox = new ConcurrentQueue<Msg>();

    /// <summary>
    /// Start to accept incoming connections
    /// </summary>
    protected override void OnStart()
    {
        Log.Info("GateManager starts...");
    }

    /// <summary>
    /// Update function called in main thread
    /// <para> Hangle queue msg (inbox and outbox) </para>
    /// </summary>
    /// <param name="dt"> delta time of the frame </param>
    protected override void DoUpdate(float dt)
    {
        ConsumeMsgInbox();
        ConsumeMsgOutbox();
        PingHeartbeat();
    }

    /// <summary>
    /// Remove all proxies
    /// <para> Stop to accept incoming connections </para>
    /// </summary>
    protected override void OnDestroy()
    {
        ResetConnection();
        Log.Info("GateManager stops...");
    }

    #region REGION_CONNECTION

    /// <summary>
    /// check if client proxy connects with the server
    /// </summary>
    /// <returns></returns>
    public bool CheckConnected()
    {
        return (proxy != null && proxy.IsConnected());
    }

    /// <summary>
    /// Reset the current proxy
    /// </summary>
    public void ResetConnection(string _ = "")
    {
        proxy?.Destroy();
        proxy = null;
        Game.Instance.GetManager<EventManager>()?.TriggerGlobalEvent("Disconnected");
    }

    public void StartConnection()
    {
        if (isConnecting)
        {
            Log.Error("[GateManager][StartConnection] Already connecting...");
            return;
        }

        ResetConnection();
        _ = Task.Run(() => StartConnectionAsync());
    }

    private async Task StartConnectionAsync()
    {
        TcpClient client = new TcpClient();
        try
        {
            await client.ConnectAsync("localhost", Const.Port);
            proxy = new Proxy(client);
            proxy.Start(
                OnReceiveMsg,
                ResetConnection
            );
            Game.Instance.GetManager<EventManager>()?.TriggerGlobalEvent("Connected");
            Log.Info("[GateManager][OnConnectionSuccess] Connected to gate server");
        }
        catch (Exception ex)
        {
            client.Close();
            Log.Error($"[GateManager][StartConnection] Failed to connect to gate server: {ex}");
        }
        finally
        {
            isConnecting = false;
        }
    }

    #endregion

    #region REGION_MSG

    /// <summary>
    /// On proxy receiving invalid msg
    /// </summary>
    /// <param name="proxyId"> proxy id </param>
    /// <param name="msg"> message </param>
    private void OnReceiveMsg(string proxyId, Msg? msg)
    {
        if (msg == null) return;
        msgInbox.Enqueue(msg);
    }

    /// <summary>
    /// Consume and handle received msg (in main thread)
    /// </summary>
    private void ConsumeMsgInbox()
    {
        int cnt = 0;
        while (cnt < Const.MsgReceiveCntPerUpdate && msgInbox.TryDequeue(out Msg? msg))
        {
            if (msg == null) continue;
            cnt++;
            InvokeRpc(msg);
        }
    }

    /// <summary>
    /// Append msg ready to be sent
    /// </summary>
    /// <param name="msg"> message </param>
    private void AppendSendMsg(Msg msg)
    {
        msgOutbox.Enqueue(msg);
    }

    /// <summary>
    /// Consume and handle msg pending for sending (in main thread)
    /// </summary>
    private void ConsumeMsgOutbox()
    {
        int cnt = 0;
        while (cnt < Const.MsgSendCntPerUpdate && msgOutbox.TryDequeue(out Msg? msg))
        {
            if (msg == null) continue;
            cnt++;
            SendMsg(msg);
        }
    }

    /// <summary>
    /// Send msg to server (in main thread)
    /// </summary>
    /// <param name="msg"> message </param>
    private void SendMsg(Msg msg)
    {
        if (!CheckConnected()) return;
        MsgStreamer.WriteMsgToStream(msg, proxy);
    }

    #endregion

    #region REGION_HEARTBEAT

    /// <summary>
    /// ping heartbeat to server
    /// </summary>
    private void PingHeartbeat()
    {
        if (!CheckConnected()) return;

        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (now - lastHeartbeatTime < Const.HeartBeatInterval) return;
        lastHeartbeatTime = now;
        CallRpc("GateManager.PingHeartbeatRemote", "Mgr-GateManager");
    }

    #endregion

    #region REGION_RPC

    public bool CallRpc(string methodName, string instanceId, params Node[] args)
    {
        if (!CheckConnected()) return false;
        AppendSendMsg(new Msg(methodName, instanceId, args));
        return true;
    }

    /// <summary>
    /// invoke remote call from server
    /// </summary>
    /// <param name="msg"> message </param>
    private void InvokeRpc(Msg msg)
    {
        // get rpc method
        string methodName = msg.methodName;
        RpcMethodInfo? rpcMethodInfo = Reflection.GetRpcMethod(methodName);
        if (rpcMethodInfo == null) return;

        // get method owner and instance
        var (owner, instance) = GetRpcOwnerAndInstance(msg.instanceId);
        if (owner == null || instance == null) return;

        // pack and invoke method
        try
        {
            rpcMethodInfo.Invoke(instance, msg.args.ToArray());
        }
        catch (Exception)
        {
            Log.Error($"Rpc method ({msg.methodName}) of instance ({instance}) failed...");
        }
    }

    #endregion
}

#if DEBUG
#region REGION_GM

[RegisterGm]
public static class GmStartConnection
{
    public static void Execute()
    {
        Game.Instance.GetManager<GateManager>()?.StartConnection();
    }
}

[RegisterGm]
public static class GmResetConnection
{
    public static void Execute()
    {
        Game.Instance.GetManager<GateManager>()?.ResetConnection();
    }
}

#endregion
#endif
