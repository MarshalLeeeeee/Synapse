using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Game : GameCommon
{
    public static Game Instance { get; private set; } = null!;

    public Game()
    {
        Instance = this;
        InitManagers();
    }

    #region REGION_API

    /// <summary>
    /// Call rpc method
    /// </summary>
    /// <param name="methodName"> name of the remote name </param>
    /// <param name="instanceId"> instance id </param>
    /// <param name="args"> optional arg list </param>
    /// <returns> Return true if the call quest is appended to the queue </returns>
    public bool CallRpc(string methodName, string instanceId, params Node[] args)
    {
        GateManager? gateManager = GetManager<GateManager>();
        if (gateManager == null) return false;
        return gateManager.CallRpc(methodName, instanceId, args);
    }

    #endregion
}