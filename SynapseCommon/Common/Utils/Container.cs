using System;
using System.Collections;
using System.Collections.Generic;

public class DoubleRefDictionary<T, U>
{
    private Dictionary<T, U> t2u = new Dictionary<T, U>();
    private Dictionary<U, T> u2t = new Dictionary<U, T>();

    /* Add new tu pair, only when both t and u are not managed by the container.
     * Return true only if write acctually performed
     */
    public bool Add(T t, U u)
    {
        if (t2u.ContainsKey(t)) return false;
        if (u2t.ContainsKey(u)) return false;

        t2u[t] = u;
        u2t[u] = t;
        return true;
    }

    /* Remove tu pair by t, only when t is managed by the container
     * Return true only if write acctually performed
     */
    public bool RemoveT(T t)
    {
        if (t2u.TryGetValue(t, out U? u))
        {
            if (u != null)
            {
                t2u.Remove(t);
                u2t.Remove(u);
                return true;
            }
            return false;
        }
        return false;
    }

    /* Remove tu pair by u, only when u is managed by the container
     * Return true only if write acctually performed
     */
    public bool RemoveU(U u)
    {
        if (u2t.TryGetValue(u, out T? t))
        {
            if (t != null)
            {
                t2u.Remove(t);
                u2t.Remove(u);
                return true;
            }
            return false;
        }
        return false;
    }

    public U? GetUByT(T t)
    {
        if (t2u.TryGetValue(t, out U? u))
        {
            return u;
        }
        return default(U?);
    }

    public T? GetTByU(U u)
    {
        if (u2t.TryGetValue(u, out T? t))
        {
            return t;
        }
        return default(T?);
    }

    public override string ToString()
    {
        string s = "DoubleRefDictionary:\n";
        if (t2u.Count == 0)
        {
            s += ">>> Empty";
        }
        else
        {
            foreach (var kvp in t2u)
            {
                s += $">>> t({kvp.Key}) with u({kvp.Value})\n";
            }
        }
        return s;
    }
}

#if DEBUG

#region REGION_TEST_DOUBLE_REF_DICTIONARY

[RegisterTest]
public static class TestDoubleRefDictionary
{
    public static void TestAddRemove()
    {
        DoubleRefDictionary<string, int> tu = new DoubleRefDictionary<string, int>();
        tu.Add("a", 1);
        int? ai = tu.GetUByT("a");
        Assert.EqualTrue(ai == 1, "ai should be 1");
        int? bi = tu.GetUByT("b");
        Assert.EqualTrue(bi == null, "bi should be null");
        string? s1 = tu.GetTByU(1);
        Assert.EqualTrue(s1 == "a", "s1 should be a");
        string? s2 = tu.GetTByU(2);
        Assert.EqualTrue(s2 == null, "s2 should be null");

        // should remain same after add
        tu.Add("a", 2);
        ai = tu.GetUByT("a");
        Assert.EqualTrue(ai == 1, "ai should be 1");
        bi = tu.GetUByT("b");
        Assert.EqualTrue(bi == null, "bi should be null");
        s1 = tu.GetTByU(1);
        Assert.EqualTrue(s1 == "a", "s1 should be a");
        s2 = tu.GetTByU(2);
        Assert.EqualTrue(s2 == null, "s2 should be null");

        // should remain same after add
        tu.Add("b", 1);
        ai = tu.GetUByT("a");
        Assert.EqualTrue(ai == 1, "ai should be 1");
        bi = tu.GetUByT("b");
        Assert.EqualTrue(bi == null, "bi should be null");
        s1 = tu.GetTByU(1);
        Assert.EqualTrue(s1 == "a", "s1 should be a");
        s2 = tu.GetTByU(2);
        Assert.EqualTrue(s2 == null, "s2 should be null");

        // should add new pair
        tu.Add("b", 2);
        ai = tu.GetUByT("a");
        Assert.EqualTrue(ai == 1, "ai should be 1");
        bi = tu.GetUByT("b");
        Assert.EqualTrue(bi == 2, "bi should be 2");
        s1 = tu.GetTByU(1);
        Assert.EqualTrue(s1 == "a", "s1 should be a");
        s2 = tu.GetTByU(2);
        Assert.EqualTrue(s2 == "b", "s2 should be b");

        // should remove succ
        tu.RemoveT("a");
        ai = tu.GetUByT("a");
        Assert.EqualTrue(ai == null, "ai should be null");
        bi = tu.GetUByT("b");
        Assert.EqualTrue(bi == 2, "bi should be 2");
        s1 = tu.GetTByU(1);
        Assert.EqualTrue(s1 == null, "s1 should be null");
        s2 = tu.GetTByU(2);
        Assert.EqualTrue(s2 == "b", "s2 should be b");

        // should remove succ
        tu.RemoveU(2);
        ai = tu.GetUByT("a");
        Assert.EqualTrue(ai == null, "ai should be null");
        bi = tu.GetUByT("b");
        Assert.EqualTrue(bi == null, "bi should be null");
        s1 = tu.GetTByU(1);
        Assert.EqualTrue(s1 == null, "s1 should be null");
        s2 = tu.GetTByU(2);
        Assert.EqualTrue(s2 == null, "s2 should be null");
    }
}

#endregion

#endif
