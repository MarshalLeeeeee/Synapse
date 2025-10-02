using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameCommon : IDisposable
{
    protected Dictionary<string, Manager> managers = new Dictionary<string, Manager>();

    /// <summary>
    /// If the game still runs in loop
    /// </summary>
    private bool isRunning;

    /// <summary>
    /// Delta time between frames
    /// </summary>
    public float dt { get; private set; }

    public void Start()
    {
        StartManagers();
        isRunning = true;
        Log.Info("Game started...");

        // ctrl c in termin to stop game
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            isRunning = false;
        };

        // Main game loop
        long nextTickTime = 0;
        while (isRunning)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (currentTime >= nextTickTime)
            {
                dt = (float)(currentTime - (nextTickTime - Const.TickInterval)) / 1000f;
                UpdateManagers(dt);
                nextTickTime = currentTime + Const.TickInterval;
            }
            else
            {
                Thread.Sleep((int)(nextTickTime - currentTime));
            }
        }

        Log.Info("Game stopped...");
    }

    public void Destroy()
    {
        isRunning = false;
        DestroyManagers();
    }

    public void Dispose()
    {
        isRunning = false;
        Destroy();
    }

    #region REGION_MANAGERS

    protected void InitManagers()
    {
        List<string> mgrNames = Reflection.GetAllManagerNames();
        foreach (string mgrName in mgrNames)
        {
            Manager? mgr = Reflection.CreateManager(mgrName);
            if (mgr != null)
            {
                managers[mgrName] = mgr;
            }
        }
    }

    public Manager? GetManager(string name)
    {
        if (managers.TryGetValue(name, out Manager? manager))
        {
            return manager;
        }
        return null;
    }

    public T? GetManager<T>() where T : Manager
    {
        Type tType = typeof(T);
        string tName = tType.Name;
        Manager? mgr = GetManager(tName);
        if (mgr != null && mgr is T tMgr)
        {
            return tMgr;
        }
        return null;
    }

    protected void StartManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Start();
        }
    }

    protected void UpdateManagers(float dt)
    {
        foreach (var manager in managers.Values)
        {
            manager.Update(dt);
        }
    }

    protected void DestroyManagers()
    {
        foreach (var manager in managers.Values)
        {
            manager.Destroy();
        }
    }

    #endregion
}