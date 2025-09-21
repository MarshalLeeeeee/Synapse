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

    #endregion

    #region REGION_LOGIN_LOGOUT

    [Rpc(RpcConst.AnyClient, NodeConst.TypeString, NodeConst.TypeString)]
    public void LoginRemote(Proxy proxy, StringNode account, StringNode password)
    {
        if (Add(proxy.proxyId, account.Get()))
        {
            Log.Info($"Account ({account.Get()}) with proxy ({proxy.proxyId}) successfully login...");
        }
    }

    [Rpc(RpcConst.AnyClient)]
    public void LogoutRemote(Proxy proxy)
    {
        if (RemoveProxyId(proxy.proxyId))
        {
            Log.Info($"Proxy ({proxy.proxyId}) successfully logout...");
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