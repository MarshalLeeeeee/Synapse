
[RegisterManager]
public class AccountManager : AccountManagerCommon
{
    /// <summary>
    /// Current login account
    /// <para> If not login, loginAccount is an empty string. </para>
    /// </summary>
    private string loginAccount = "";

    /// <summary>
    /// True only if after calling LoginRemote and before receiving result
    /// </summary>
    private bool waitLoginRes = false;

    /// <summary>
    /// True only if after calling LogoutRemote and before receiving result
    /// </summary>
    private bool waitLogoutRes = false;

    protected override void OnStart()
    {
        Game.Instance.GetManager<EventManager>()?.RegisterGlobalEvent("Disconnected", "AccountManager.Reset", Reset);
    }

    protected override void DoUpdate(float dt) { }

    protected override void OnDestroy()
    {
        Game.Instance.GetManager<EventManager>()?.UnregisterGlobalEvent("Disconnected", "AccountManager.Reset");
    }

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
            Log.Info("A login remote invoke is in process");
            return false;
        }

        if (Game.Instance.CallRpc("AccountManager.LoginRemote", "Mgr-AccountManager", new StringNode(account), new StringNode(password)))
        {
            waitLoginRes = true;
            Log.Info("Login remote invoke starts");
            return true;
        }
        else
        {
            Log.Info("Login remote invoke fails due to abnormal connection");
            return false;
        }
    }

    [Rpc(RpcConst.Server)]
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
        Game.Instance.GetManager<EventManager>()?.TriggerGlobalEvent("OnLogin");
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

        if (Game.Instance.CallRpc("AccountManager.LogoutRemote", "Mgr-AccountManager"))
        {
            waitLogoutRes = true;
            Log.Info("Logout remote invoke starts");
            return true;
        }
        else
        {
            Log.Info("Logout remote invoke fails due to abnormal connection");
            return false;
        }
    }

    [Rpc(RpcConst.Server)]
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
            Game.Instance.GetManager<EventManager>()?.TriggerGlobalEvent("OnLogout");
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

    /// <summary>
    /// reset all login or pending state
    /// </summary>
    private void Reset()
    {
        loginAccount = "";
        waitLoginRes = false;
        waitLogoutRes = false;
        Game.Instance.GetManager<EventManager>()?.TriggerGlobalEvent("OnLogout");
        Log.Info("[AccountManager][Reset] reset over...");
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