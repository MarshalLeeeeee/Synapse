using System;
using System.Collections;
using System.Collections.Concurrent;

[RegisterManager]
public class AccountManager : AccountManagerCommon
{
    /* Manager proxy with login account.
     * When proxy remains valid, changes happen when corresponding client invoke login or logout remotely.
     * When proxy becomes invalid, corresponding account automatically logout.
     */
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

    /* Add proxyId account pair
     * Return true only if write acctually performed
     */
    private bool Add(string proxyId, string account)
    {
        return proxyIdWithAccount.Add(proxyId, account);
    }

    /* Remove proxyId account pair by proxyId
     * Return true only if write acctually performed
     */
    private bool RemoveProxyId(string proxyId)
    {
        return proxyIdWithAccount.RemoveT(proxyId);
    }

    /* Remove proxyId account pair by account
     * Return true only if write acctually performed
     */
    private bool RemoveAccount(string account)
    {
        return proxyIdWithAccount.RemoveU(account);
    }

    /* Get account by proxy id
     * If not exist, return null
     */
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

    [Rpc(RpcConst.AnyClient, NodeConst.TypeString, NodeConst.TypeString)]
    public void LoginRemote(Proxy proxy, StringNode account, StringNode password)
    {
        string proxyId = proxy.proxyId;
        string accountValue = account.Get();
        if (Add(proxyId, accountValue))
        {
            NotifyLoginSucc(proxyId, accountValue);
            Log.Info($"Account ({accountValue}) with proxy ({proxyId}) successfully login...");
        }
        else
        {
            NotifyLoginFail(proxyId);
            Log.Info($"Account ({accountValue}) with proxy ({proxyId}) fails to login...");
        }
    }

    /* notify corresponding client of login succ */
    private void NotifyLoginSucc(string proxyId, string account)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            gateMgr.CallRpc(proxyId, "LoginResRemote", "AccountManager", "", new StringNode(account));
        }
    }

    /* notify corresponding client of login fail */
    private void NotifyLoginFail(string proxyId)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            gateMgr.CallRpc(proxyId, "LoginResRemote", "AccountManager", "", new StringNode(""));
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

    /* notify corresponding client of logout succ */
    private void NotifyLogoutSucc(string proxyId, string account)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            gateMgr.CallRpc(proxyId, "LogoutResRemote", "AccountManager", "", new StringNode(account));
        }
    }

    /* notify corresponding client of logout fail */
    private void NotifyLogoutFail(string proxyId)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr != null)
        {
            gateMgr.CallRpc(proxyId, "LogoutResRemote", "AccountManager", "", new StringNode(""));
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