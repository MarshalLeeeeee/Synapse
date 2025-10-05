
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
    /// <param name="player"> player entity instance </param>
    [Rpc(RpcConst.Server, NodeTypeConst.TypePlayerEntity)]
    public void SyncAddPlayerRemote(PlayerEntity player)
    {
        string playerId = player.entityId;
        if (!String.IsNullOrEmpty(playerId))
        {
            playerEntities.Add(playerId, player);
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
