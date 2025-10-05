
using System.Security.Cryptography;

[RegisterManager]
public class EntityManager : EntityManagerCommon
{
    /// <summary>
    /// manage account with corresponding player entity id
    /// </summary>
    private DoubleRefDictionary<string, string> accountWithPlayerId = new DoubleRefDictionary<string, string>();

    protected override void OnStart()
    {
        Game.Instance.GetManager<EventManager>()?.RegisterGlobalEvent<string, string>("OnLogin", "EntityManager.EnsurePlayerEntity", EnsurePlayerEntity);
    }

    protected override void DoUpdate(float dt) { }

    protected override void OnDestroy()
    {
        Game.Instance.GetManager<EventManager>()?.UnregisterGlobalEvent("OnLogin", "EntityManager.EnsurePlayerEntity");
    }

    /// <summary>
    /// ensure player entity with the given accont
    /// <para> if there is no corresponding player entity, create it </para>
    /// <para> if there is corresponding player entity, reuse it </para>
    /// </summary>
    /// <param name="proxyId"> proxy id </param>
    /// <param name="account"> account </param>
    public void EnsurePlayerEntity(string proxyId, string account)
    {
        if (accountWithPlayerId.GetUByT(account, out string? playerId))
        {
            if (playerId != null)
            {
                SyncAllPlayerEntity(proxyId);
            }
            else
            {
                CreatePlayerEntity(proxyId, account);
            }
        }
        else
        {
            CreatePlayerEntity(proxyId, account);
        }
    }

    /// <summary>
    /// create player with the given acconut
    /// </summary>
    /// <param name="account"> account </param>
    private void CreatePlayerEntity(string proxyId, string account)
    {
        PlayerEntity player = new PlayerEntity();
        string playerId = player.entityId;
        playerEntities.Add(playerId, player);
        accountWithPlayerId.Add(account, playerId);
        GateManager? gateManager = Game.Instance.GetManager<GateManager>();
        if (gateManager != null)
        {
            List<string> proxyIds = gateManager.GetProxyIds();
            foreach (string pid in proxyIds)
            {
                if (pid == proxyId)
                {
                    SyncAllPlayerEntity(pid);
                }
                else
                {
                    SyncPlayerEntity(pid, playerId);
                }
            }
        }
    }

    /// <summary>
    /// synchronize all player entities to target client
    /// </summary>
    /// <param name="proxyId"> proxy id </param>
    public void SyncAllPlayerEntity(string proxyId)
    {
        string? targetPlayerId = GetPlayerIdByProxyId(proxyId);
        if (targetPlayerId == null) return;

        foreach (var kvp in playerEntities)
        {
            string playerId = kvp.Key;
            PlayerEntity sourcePlayer = kvp.Value;
            if (targetPlayerId == sourcePlayer.entityId)
            {
                DoSyncMainPlayerEntity(proxyId, sourcePlayer);
            }
            else
            {
                DoSyncOtherPlayerEntity(proxyId, sourcePlayer);
            }
        }
    }

    /// <summary>
    /// sync full entity to target client
    /// </summary>
    /// <param name="proxyId"> proxy id of the target client </param>
    /// <param name="playerId"> id of the player entity to be synchronized </param>
    public void SyncPlayerEntity(string proxyId, string playerId)
    {
        PlayerEntity? sourcePlayer = GetPlayerEntity(playerId);
        if (sourcePlayer == null) return;

        string? targetPlayerId = GetPlayerIdByProxyId(proxyId);
        if (targetPlayerId == null) return;

        if (targetPlayerId == sourcePlayer.entityId)
        {
            DoSyncMainPlayerEntity(proxyId, sourcePlayer);
        }
        else
        {
            DoSyncOtherPlayerEntity(proxyId, sourcePlayer);
        }
    }

    /// <summary>
    /// sync player entity to target proxy with all but not own fields
    /// </summary>
    /// <param name="proxyId"> proxy id</param>
    /// <param name="player"> player entity to be synced </param>
    private static void DoSyncOtherPlayerEntity(string proxyId, PlayerEntity player)
    {
        Game.Instance.CallRpc(proxyId, "EntityManager.SyncAddPlayerRemote", "Mgr-EntityManager", player);
    }

    /// <summary>
    /// sync player entity to target proxy with all but not own fields
    /// </summary>
    /// <param name="proxyId"></param>
    /// <param name="player"></param>
    private static void DoSyncMainPlayerEntity(string proxyId, PlayerEntity player)
    {
        Game.Instance.CallRpc(proxyId, "EntityManager.SyncAddPlayerRemote", "Mgr-EntityManager", player);
    }

    /// <summary>
    /// get player id of the coorresponding account
    /// </summary>
    /// <param name="account"> account </param>
    /// <returns> Return playerId if exists, null otherwise </returns>
    public string? GetPlayerIdByAccount(string account)
    {
        if (accountWithPlayerId.GetUByT(account, out string? playerId))
        {
            return playerId;
        }
        return null;
    }

    /// <summary>
    /// get player id of the corresponding proxy id
    /// </summary>
    /// <param name="proxyId"> proxy id </param>
    /// <returns> Return playerId if exists, null otherwise </returns>
    public string? GetPlayerIdByProxyId(string proxyId)
    {
        string? account = Game.Instance.GetManager<AccountManager>()?.GetAccount(proxyId);
        if (account == null) return null;

        return GetPlayerIdByAccount(account);
    }
}
