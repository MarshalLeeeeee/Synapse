
using System.Collections.Generic;

[RegisterManager]
public class EntityManager : EntityManagerCommon
{
    /// <summary>
    /// id of the main player entity
    /// </summary>
    string mainPlayerId = "";

    #region REGION_SYNC_REMOTE

    /// <summary>
    /// add player by server synchronization
    /// </summary>
    /// <param name="player"> player entity instance (whose id should be empty) </param>
    [Rpc(RpcConst.Server, NodeTypeConst.TypeString, NodeTypeConst.TypePlayerEntity)]
    public void SyncAddPlayerRemote(StringNode playerId, PlayerEntity player)
    {
        string playerId_ = playerId.Get();
        if (!String.IsNullOrEmpty(playerId_))
        {
            playerEntities[playerId_] = player;
            player.SetId(playerId_);
        }
    }

    /// <summary>
    /// remove player by server synchronization
    /// </summary>
    /// <param name="playerId"> player id wrapped by StringNode </param>
    [Rpc(RpcConst.Server, NodeTypeConst.TypePlayerEntity)]
    public void SyncRemovePlayerRemote(StringNode playerId)
    {
        string playerId_ = playerId.Get();
        playerEntities.Remove(playerId_);
    }

    #endregion
}
