
[RegisterManager]
public class AccountManager : AccountManagerCommon
{
    #region REGION_LOGIN_LOGOUT

    public bool Login(string account, string password)
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr == null) return false;

        gateMgr.CallRpc("LoginRemote", "AccountManager", "", new StringNode(account), new StringNode(password));
        return true;
    }

    public bool Logout()
    {
        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr == null) return false;

        gateMgr.CallRpc("LogoutRemote", "AccountManager", "");
        return true;
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