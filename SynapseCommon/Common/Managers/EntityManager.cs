
/// <summary>
/// Manager all entities
/// </summary>
public class EntityManagerCommon : Manager
{
    protected Dictionary<string, PlayerEntity> playerEntities = new Dictionary<string, PlayerEntity>();

    public override string ToString()
    {
        string s = "";
        foreach (var kvp in playerEntities)
        {
            s += $">>>> Id({kvp.Key}): {kvp.Value}\n";
        }
        if (String.IsNullOrEmpty(s)) s = ">>>> Empty";
        s = $">> Player entities: \n{s}\n";
        return s;
    }

    /// <summary>
    /// get player entity by player entity id
    /// </summary>
    /// <param name="playerId"> id of the player entity </param>
    /// <returns> Return PlayerEntity instance if exists, null otherwise </returns>
    public PlayerEntity? GetPlayerEntity(string playerId)
    {
        if (playerEntities.TryGetValue(playerId, out PlayerEntity? playerEntity))
        {
            return playerEntity;
        }
        return null;
    }

    /// <summary>
    /// destroy all managed entities
    /// </summary>
    public void DestroyAllEntities()
    {
        // destroy all player entities
        playerEntities.Clear();
    }
}

#if DEBUG

[RegisterGm]
public static class GmShowPlayerEntities
{
    public static void Execute()
    {
        Log.Debug($"{Game.Instance.GetManager<EntityManager>()}");
    }
}

#endif
