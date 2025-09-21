using System;
using System.Collections;
using System.Collections.Concurrent;

[RegisterManager]
public class AccountManager : AccountManagerCommon
{
    private DoubleRefDictionary<string, string> proxyIdWithAccount = new DoubleRefDictionary<string, string>();


    protected override void OnStart() { }

    protected override void DoUpdate(float dt) { }

    protected override void OnDestroy() { }

    public override string ToString()
    {
        return proxyIdWithAccount.ToString();
    }

    #region REGION_DATA

    private void Add(string proxyId, string account)
    {
        proxyIdWithAccount.Add(proxyId, account);
    }

    private void RemoveProxyId(string proxyId)
    {
        proxyIdWithAccount.RemoveT(proxyId);
    }

    private void RemoveAccount(string account)
    {
        proxyIdWithAccount.RemoveU(account);
    }

    #endregion

    #region REGION_LOGIN_LOGOUT

    [Rpc(RpcConst.AnyClient, NodeConst.TypeString, NodeConst.TypeString)]
    public void LoginRemote(Proxy proxy, StringNode account, StringNode password)
    {
        Add(proxy.proxyId, account.Get());
        Log.Info($"Account ({account.Get()}) with proxy ({proxy.proxyId}) successfully login...");
    }

    [Rpc(RpcConst.AnyClient)]
    public void LogoutRemote(Proxy proxy)
    {
        RemoveProxyId(proxy.proxyId);
        Log.Info($"Proxy ({proxy.proxyId}) successfully logout...");
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