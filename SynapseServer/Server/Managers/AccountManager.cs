using System;
using System.Collections;
using System.Collections.Concurrent;

[RegisterManager]
public class AccountManager : AccountManagerCommon
{
    /// <summary>
    /// Manager proxy with login account.
    /// <para> When proxy remains valid, changes happen when corresponding client invoke login or logout remotely. </para>
    /// <para> When proxy becomes invalid, corresponding account automatically logout. </para>
    /// </summary>
    private DoubleRefDictionary<string, string> proxyIdWithAccount = new DoubleRefDictionary<string, string>();

    protected override void OnStart()
    {
        EventManager? eventManager = Game.Instance.GetManager<EventManager>();
        if (eventManager != null)
        {
            eventManager.RegisterGlobalEvent<string>("OnRemoveProxy", "OnRemoveProxy", OnRemoveProxy);
        }
    }

    protected override void DoUpdate(float dt) { }

    protected override void OnDestroy()
    {
        EventManager? eventManager = Game.Instance.GetManager<EventManager>();
        if (eventManager != null)
        {
            eventManager.UnregisterGlobalEvent("OnRemoveProxy", "OnRemoveProxy");
        }
    }

    public override string ToString()
    {
        return proxyIdWithAccount.ToString();
    }

    #region REGION_DATA

    /// <summary>
    /// Add proxyId account pair
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    /// <param name="account"> account name </param>
    /// <returns> Return true only if modification acctually performed </returns>
    private bool AddAccount(string proxyId, string account)
    {
        return proxyIdWithAccount.Add(proxyId, account);
    }

    /// <summary>
    /// Remove proxyId account pair by proxyId
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    /// <returns> Return true only if modification acctually performed </returns>
    private bool RemoveProxyId(string proxyId)
    {
        return proxyIdWithAccount.RemoveT(proxyId);
    }

    /// <summary>
    /// Remove proxyId account pair by account
    /// </summary>
    /// <param name="account"> account name </param>
    /// <returns> Return true only if modification acctually performed </returns>
    private bool RemoveAccount(string account)
    {
        return proxyIdWithAccount.RemoveU(account);
    }

    /// <summary>
    /// Get account by proxy id
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    /// <returns> Account or null </returns>
    public string? GetAccount(string proxyId)
    {
        string? account;
        if (proxyIdWithAccount.GetUByT(proxyId, out account))
        {
            return account;
        }
        return null;
    }

    #endregion

    #region REGION_LOGIN_LOGOUT

    [Rpc(RpcConst.AnyClient, NodeTypeConst.TypeString, NodeTypeConst.TypeString)]
    public void LoginRemote(Proxy proxy, StringNode account, StringNode password)
    {
        string proxyId = proxy.proxyId;
        string accountValue = account.Get();
        if (AddAccount(proxyId, accountValue))
        {
            EnsurePlayerEntity(accountValue);
            NotifyLoginSucc(proxyId, accountValue);
            Log.Info($"Account ({accountValue}) with proxy ({proxyId}) successfully login...");
        }
        else
        {
            NotifyLoginFail(proxyId);
            Log.Info($"Account ({accountValue}) with proxy ({proxyId}) fails to login...");
        }
    }

    /// <summary>
    /// ensure player entity with the given accont
    /// </summary>
    /// <param name="account"> account </param>
    private void EnsurePlayerEntity(string account)
    {
        EntityManager? entityManager = Game.Instance.GetManager<EntityManager>();
        if (entityManager != null)
        {
            entityManager.EnsurePlayerEntity(account);
        }
    }

    /// <summary>
    /// notify corresponding client of login succ
    /// <para> sync Nodes to client </para>
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    /// <param name="account"> account name </param>
    private static void NotifyLoginSucc(string proxyId, string account)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            // TODO: here should be a full sync of Node from server to client
            gateMgr.CallRpc(proxyId, "AccountManager.LoginResRemote", "AccountManager", "", new StringNode(account));
        }
    }

    /// <summary>
    /// notify corresponding client of login fail
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    private static void NotifyLoginFail(string proxyId)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            gateMgr.CallRpc(proxyId, "AccountManager.LoginResRemote", "AccountManager", "", new StringNode(""));
        }
    }

    [Rpc(RpcConst.AnyClient)]
    public void LogoutRemote(Proxy proxy)
    {
        string proxyId = proxy.proxyId;
        string? account = GetAccount(proxyId);
        if (account != null)
        {
            if (RemoveAccount(account))
            {
                NotifyLogoutSucc(proxyId, account);
                Log.Info($"Proxy ({proxyId}) successfully logout...");
            }
            else
            {
                NotifyLogoutFail(proxyId);
                Log.Info($"Proxy ({proxyId}) fails to logout...");
            }
        }
        else
        {
            NotifyLogoutFail(proxyId);
            Log.Info($"Proxy ({proxyId}) fails to logout because corresponding account is null...");
        }
    }

    /// <summary>
    /// notify corresponding client of logout succ
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    /// <param name="account"> account name </param>
    private static void NotifyLogoutSucc(string proxyId, string account)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            gateMgr.CallRpc(proxyId, "AccountManager.LogoutResRemote", "AccountManager", "", new StringNode(account));
        }
    }

    /// <summary>
    /// notify corresponding client of logout fail
    /// </summary>
    /// <param name="proxyId"> id of the proxy </param>
    private static void NotifyLogoutFail(string proxyId)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            gateMgr.CallRpc(proxyId, "AccountManager.LogoutResRemote", "AccountManager", "", new StringNode(""));
        }
    }

    private void OnRemoveProxy(string proxyId)
    {
        if (RemoveProxyId(proxyId))
        {
            Log.Info($"Proxy ({proxyId}) successfully logout on proxy removed...");
        }
    }

    #endregion
}

#if DEBUG

#region REGION_GM

[RegisterGm]
public static class GmShowAccounts
{
    public static void Execute()
    {
        Log.Debug($"Account info: \n {Game.Instance.GetManager<AccountManager>()}");
    }
}

#endregion

#endif