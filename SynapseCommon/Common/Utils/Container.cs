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

    public bool GetUByT(T t, out U? u)
    {
        if (t2u.TryGetValue(t, out U? uu))
        {
            u = uu;
            return true;
        }
        u = default(U?);
        return false;
    }

    public bool GetTByU(U u, out T? t)
    {
        if (u2t.TryGetValue(u, out T? tt))
        {
            t = tt;
            return true;
        }
        t = default(T?);
        return false;
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
        string? t;
        int u;

        Assert.EqualTrue(tu.Add("a", 1), "First add (a, 1) should be ok");
        Assert.EqualTrue(tu.GetUByT("a", out u), "First get by a should be ok");
        Assert.EqualTrue(u == 1, "should be 1");
        Assert.EqualFalse(tu.GetUByT("b", out u), "First get by b should be wrong");
        Assert.EqualTrue(tu.GetTByU(1, out t), "First get by 1 should be ok");
        Assert.EqualTrue(t == "a", "should be a");
        Assert.EqualFalse(tu.GetTByU(2, out t), "First get by 2 should be wrong");

        // should remain same after add
        Assert.EqualFalse(tu.Add("a", 2), "Second add (a, 2) should be wrong");
        Assert.EqualTrue(tu.GetUByT("a", out u), "Second get by a should be ok");
        Assert.EqualTrue(u == 1, "should be 1");
        Assert.EqualFalse(tu.GetUByT("b", out u), "Second get by b should be wrong");
        Assert.EqualTrue(tu.GetTByU(1, out t), "Second get by 1 should be ok");
        Assert.EqualTrue(t == "a", "should be a");
        Assert.EqualFalse(tu.GetTByU(2, out t), "Second get by 2 should be wrong");

        // should remain same after add
        Assert.EqualFalse(tu.Add("b", 1), "Third add (b, 1) should be wrong");
        Assert.EqualTrue(tu.GetUByT("a", out u), "Third get by a should be ok");
        Assert.EqualTrue(u == 1, "should be 1");
        Assert.EqualFalse(tu.GetUByT("b", out u), "Third get by b should be wrong");
        Assert.EqualTrue(tu.GetTByU(1, out t), "Third get by 1 should be ok");
        Assert.EqualTrue(t == "a", "should be a");
        Assert.EqualFalse(tu.GetTByU(2, out t), "Third get by 2 should be wrong");

        // should add new pair
        Assert.EqualTrue(tu.Add("b", 2), "Fourth add (b, 2) should be ok");
        Assert.EqualTrue(tu.GetUByT("a", out u), "Fourth get by a should be ok");
        Assert.EqualTrue(u == 1, "should be 1");
        Assert.EqualTrue(tu.GetUByT("b", out u), "Fourth get by b should be ok");
        Assert.EqualTrue(u == 2, "should be 2");
        Assert.EqualTrue(tu.GetTByU(1, out t), "Fourth get by 1 should be ok");
        Assert.EqualTrue(t == "a", "should be a");
        Assert.EqualTrue(tu.GetTByU(2, out t), "Fourth get by 2 should be ok");
        Assert.EqualTrue(t == "b", "should be b");

        // should remove succ
        Assert.EqualTrue(tu.RemoveT("a"), "Fifth remove by t:a should be ok");
        Assert.EqualFalse(tu.GetUByT("a", out u), "Fifth get by a should be wrong");
        Assert.EqualTrue(tu.GetUByT("b", out u), "Fifth get by b should be ok");
        Assert.EqualTrue(u == 2, "should be 2");
        Assert.EqualFalse(tu.GetTByU(1, out t), "Fifth get by 1 should be wrong");
        Assert.EqualTrue(tu.GetTByU(2, out t), "Fifth get by 2 should be ok");
        Assert.EqualTrue(t == "b", "should be b");

        // should remove fail
        Assert.EqualFalse(tu.RemoveT("a"), "Sixth remove by t:a should be wrong");
        Assert.EqualFalse(tu.GetUByT("a", out u), "Sixth get by a should be wrong");
        Assert.EqualTrue(tu.GetUByT("b", out u), "Sixth get by b should be ok");
        Assert.EqualTrue(u == 2, "should be 2");
        Assert.EqualFalse(tu.GetTByU(1, out t), "Sixth get by 1 should be wrong");
        Assert.EqualTrue(tu.GetTByU(2, out t), "Sixth get by 2 should be ok");
        Assert.EqualTrue(t == "b", "should be b");

        // should remove succ
        Assert.EqualTrue(tu.RemoveU(2), "Seventh remove by u:2 should be ok");
        Assert.EqualFalse(tu.GetUByT("a", out u), "Seventh get by a should be wrong");
        Assert.EqualFalse(tu.GetUByT("b", out u), "Seventh get by b should be wrong");
        Assert.EqualFalse(tu.GetTByU(1, out t), "Seventh get by 1 should be wrong");
        Assert.EqualFalse(tu.GetTByU(2, out t), "Seventh get by 2 should be wrong");
    }
}

#endregion

#endif
