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
public class Proxy : ProxyCommon
{
    /* lastest time stamp of sending heartbeat */
    public long lastHeartbeatTime = 0;

    public Proxy(TcpClient client_) : base(client_) { }
}

/*
 * GateManager is responsible for communication between server and client
 */
[IsManager]
public class GateManager : GateManagerCommon
{
    private Proxy? proxy;

    /* flag to indicate if client is connecting async */
    private volatile bool isConnecting = false;

    private ConcurrentQueue<Msg> msgInbox = new ConcurrentQueue<Msg>();
    private ConcurrentQueue<Msg> msgOutbox = new ConcurrentQueue<Msg>();

        /*
     * Start to accept incoming connections
     */
    protected override void OnStart()
    {
        Log.Info("GateManager starts...");
    }

    /*
     * Update function called in main thread
     ** Hangle queue msg (inbox and outbox)
     */
    protected override void DoUpdate(float dt)
    {
        ConsumeMsgInbox();
        ConsumeMsgOutbox();
    }

    /*
     * Remove all proxies
     * Stop to accept incoming connections
     */
    protected override void OnDestroy()
    {
        ResetConnection();
        Log.Info("GateManager stops...");
    }

    #region REGION_CONNECTION

    /* Reset the current proxy */
    public void ResetConnection(string _ = "")
    {
        proxy?.Destroy();
        proxy = null;
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
            OnConnectionSuccess(client);
        }
        catch (Exception ex)
        {
            client.Close();
            OnConnectionFailure();
            Log.Error($"[GateManager][StartConnection] Failed to connect to gate server: {ex}");
        }
    }

    private void OnConnectionSuccess(TcpClient client)
    {
        isConnecting = false;
        proxy = new Proxy(client);
        proxy.Start(
            OnReceiveMsg,
            ResetConnection
        );
        Log.Info("[GateManager][OnConnectionSuccess] Connected to gate server");
    }

    private void OnConnectionFailure()
    {
        isConnecting = false;
    }

    #endregion

    #region REGION_MSG

    /* On proxy receiving invalid msg */
    private void OnReceiveMsg(string proxyId, Msg? msg)
    {
        if (msg == null) return;
        msgInbox.Enqueue(msg);
    }

    /*
     * Consume and handle received msg (in main thread)
     */
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

    /* Append msg ready to be sent */
    public void AppendSendMsg(Msg msg)
    {
        msgOutbox.Enqueue(msg);
    }

    /*
     * Consume and handle msg pending for sending (in main thread)
     */
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

    /*
     * Send msg to server (in main thread)
     */
    private void SendMsg(Msg msg)
    {
        if (!proxy.IsConnected()) return;
        MsgStreamer.WriteMsgToStream(proxy.stream, msg);
    }

    #endregion

    #region REGION_RPC

    private void InvokeRpc(Msg msg)
    {

    }

    #endregion
}
