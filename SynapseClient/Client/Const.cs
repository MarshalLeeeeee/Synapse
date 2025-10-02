
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
    /// million second of the heartbeat ping interval
    /// </summary>
    public const int HeartBeatInterval = 3000;

    /// <summary>
    /// valid type of received rpc
    /// </summary>
    public const int RpcType = RpcConst.Server;

    /// <summary>
    /// title of console window
    /// </summary>
    public const string Title = "SynapseClient";
}
