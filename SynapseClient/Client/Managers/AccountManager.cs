
[RegisterManager]
public class AccountManager : AccountManagerCommon
{
    /* current login account 
     * if not login, account is empty string
     */
    private string currAccount = "";

    #region REGION_LOGIN_LOGOUT

    public bool CheckLogin()
    {
        return !String.IsNullOrEmpty(currAccount);
    }

    public bool Login(string account, string password)
    {
        if (CheckLogin()) return false;

        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr == null) return false;

        Msg msg = new Msg("LoginRemote", "AccountManager", "");
        msg.arg.Add(new StringNode(account));
        msg.arg.Add(new StringNode(password));
        gateMgr.AppendSendMsg(msg);
        currAccount = account;
        return true;
    }

    public bool Logout()
    {
        if (!CheckLogin()) return false;

        GateManager? gateMgr = Game.Instance.GetManager<GateManager>();
        if (gateMgr == null) return false;

        Msg msg = new Msg("LogoutRemote", "AccountManager", "");
        gateMgr.AppendSendMsg(msg);
        currAccount = "";
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