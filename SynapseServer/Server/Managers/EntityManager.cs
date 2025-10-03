
public class EntityManager : EntityManagerCommon
{
    /// <summary>
    /// manage account with corresponding player entity id
    /// </summary>
    private DoubleRefDictionary<string, string> accountWithPlayerId = new DoubleRefDictionary<string, string>();

    /// <summary>
    /// ensure player entity with the given accont
    /// <para> if there is no corresponding player entity, create it </para>
    /// <para> if there is corresponding player entity, reuse it </para>
    /// </summary>
    /// <param name="account"> account </param>
    public void EnsurePlayerEntity(string account)
    {
        if (accountWithPlayerId.GetUByT(account, out string? playerId))
        {
            if (playerId != null)
            {
                // do nothing with creation, maybe cancel auto recycle timer
            }
            else
            {
                CreatePlayerEntity(account);
            }
        }
        else
        {
            CreatePlayerEntity(account);
        }
    }

    /// <summary>
    /// create player with the given acconut
    /// </summary>
    /// <param name="account"> account </param>
    private void CreatePlayerEntity(string account)
    {
        string playerId = "debug";
        accountWithPlayerId.Add(account, playerId);
    }
}
