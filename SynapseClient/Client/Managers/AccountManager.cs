
[RegisterManager]
public class AccountManager : AccountManagerCommon
{
    /* Current login account
     * If not login, loginAccount is an empty string.
     */
    private string loginAccount = "";
    /* True only if after calling LoginRemote and before receiving result */
    private bool waitLoginRes = false;
    /* True only if after calling LogoutRemote and before receiving result */
    private bool waitLogoutRes = false;

    #region REGION_LOGIN_LOGOUT

    public bool CheckLogin()
    {
        return !String.IsNullOrEmpty(loginAccount);
    }

    public bool Login(string account, string password)
    {
        if (CheckLogin())
        {
            Log.Info("Already is login state");
            return false;
        }
        if (waitLoginRes)
        {
            Log.Info("A login remote invoke is in process");
            return false;
        }
        if (waitLogoutRes)
        {
            Log.Info("A logout remote invoke is in process");
            return false;
        }
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr == null)
        {
            Log.Error("GateManager is not found");
            return false;
        }

        waitLoginRes = true;
        gateMgr.CallRpc("LoginRemote", "AccountManager", "", new StringNode(account), new StringNode(password));
        return true;
    }

    [Rpc(RpcConst.Server, NodeConst.TypeString)]
    public void LoginResRemote(StringNode account)
    {
        string a = account.Get();
        if (String.IsNullOrEmpty(a))
        {
            LoginFail();
        }
        else
        {
            LoginSucc(a);
        }
    }

    private void LoginSucc(string s)
    {
        if (!waitLoginRes)
        {
            Log.Info("[LoginSucc] Login res already dealt");
            return;
        }
        waitLoginRes = false;
        loginAccount = s;
        Log.Info($"[LoginSucc] Login as account({s})");
    }

    private void LoginFail()
    {
        if (!waitLoginRes)
        {
            Log.Info("[LoginFail] Login res already dealt");
            return;
        }
        waitLoginRes = false;
        Log.Info("[LoginSucc] Login fail");
    }

    public bool Logout()
    {
        if (!CheckLogin())
        {
            Log.Info("Already is logout state");
            return false;
        }
        if (waitLoginRes)
        {
            Log.Info("A login remote invoke is in process");
            return false;
        }
        if (waitLogoutRes)
        {
            Log.Info("A logout remote invoke is in process");
            return false;
        }
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr == null)
        {
            Log.Error("GateManager is not found");
            return false;
        }

        waitLogoutRes = true;
        gateMgr.CallRpc("LogoutRemote", "AccountManager", "");
        return true;
    }

    [Rpc(RpcConst.Server, NodeConst.TypeString)]
    public void LogoutResRemote(StringNode account)
    {
        string a = account.Get();
        if (String.IsNullOrEmpty(a))
        {
            LogoutFail();
        }
        else
        {
            LogoutSucc(a);
        }
    }

    private void LogoutSucc(string account)
    {
        if (!waitLogoutRes)
        {
            Log.Info("[LoginSucc] Logout res already dealt");
            return;
        }
        waitLogoutRes = false;
        if (account == loginAccount)
        {
            loginAccount = "";
            Log.Info("[LoginSucc] Reset local login account");
        }
        else
        {
            Log.Error("[LoginSucc] Local account is different from remote account");
        }
    }

    private void LogoutFail()
    {
        if (!waitLogoutRes)
        {
            Log.Info("[LogoutFail] Logout res already dealt");
            return;
        }
        waitLogoutRes = false;
        Log.Info("[LogoutFail] Logout fail");
    }

    #endregion
}

#if DEBUG

#region REGION_GM

[RegisterGm]
public static class GmLogin
{
    public static void Execute(string account, string password)
    {
        Game.Instance.GetManager<AccountManager>()?.Login(account, password);
    }
}

[RegisterGm]
public static class GmLogout
{
    public static void Execute()
    {
        Game.Instance.GetManager<AccountManager>()?.Logout();
    }
}

#endregion

#endif