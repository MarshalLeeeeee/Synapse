
public class Const : ConstCommon
{
    /// <summary>
    /// tick interval of Game.Update (ms)
    /// </summary>
    public const int TickInterval = 10;

    /// <summary>
    /// cnt of handled mag in one update
    /// </summary>
    public const int MsgReceiveCntPerUpdate = 1000;

    /// <summary>
    /// cnt of handled mag in one update
    /// </summary>
    public const int MsgSendCntPerUpdate = 1000;

    /// <summary>
    /// the min time interval of proxy check
    /// </summary>
    public const int CheckProxyInterval = 1000;

    /// <summary>
    /// cnt of proxy checked in one update
    /// </summary>
    public const int CheckProxyCntPerUpdate = 5;

    /// <summary>
    /// million second of the longest inactive heartbeat interval
    /// </summary>
    public const int HeartBeatThreshold = 10000;

    /// <summary>
    /// valid type of received rpc
    /// </summary>
    public const int RpcType = RpcConst.Client;

    /// <summary>
    /// title of console window
    /// </summary>
    public const string Title = "SynapseServer";
}
