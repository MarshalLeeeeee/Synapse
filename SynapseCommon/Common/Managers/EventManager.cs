using System;
using System.Collections.Generic;

public class EventCallbacks
{
    /// <summary>
    /// Callback dictionary
    /// <param> callback name -> callback delegate </param>
    /// </summary>
    private Dictionary<string, Delegate> callbacks = new Dictionary<string, Delegate>();

    /// <summary>
    /// Number of callbacks managed by the event callback manager
    /// </summary>
    public int Count => callbacks.Count;

    /// <summary>
    /// Add callback to the callback dictionary
    /// </summary>
    /// <param name="callbackName"> name of the callbcak (used as key) </param>
    /// <param name="callback"> callback delegate </param>
    public void AddCallback(string callbackName, Delegate callback)
    {
        callbacks.TryAdd(callbackName, callback);
    }

    /// <summary>
    /// Remove callback from the callback dictionary
    /// </summary>
    /// <param name="callbackName"> name of the callbcak (used as key) </param>
    public void RemoveCallback(string callbackName)
    {
        callbacks.Remove(callbackName);
    }

    /// <summary>
    /// Get all callbacks of the specified delegate type Callback()
    /// </summary>
    public IEnumerable<Action> GetCallbacks()
    {
        foreach (Delegate callback in callbacks.Values)
        {
            if (callback is Action typedCallback)
            {
                yield return typedCallback;
            }
        }
    }

    /// <summary>
    /// Get all callbacks of the specified delegate type Callback(T1)
    /// </summary>
    public IEnumerable<Action<T1>> GetCallbacks<T1>()
    {
        foreach (Delegate callback in callbacks.Values)
        {
            if (callback is Action<T1> typedCallback)
            {
                yield return typedCallback;
            }
        }
    }

    /// <summary>
    /// Get all callbacks of the specified delegate type Callback(T1, T2)
    /// </summary>
    public IEnumerable<Action<T1, T2>> GetCallbacks<T1, T2>()
    {
        foreach (Delegate callback in callbacks.Values)
        {
            if (callback is Action<T1, T2> typedCallback)
            {
                yield return typedCallback;
            }
        }
    }

    /// <summary>
    /// Get all callbacks of the specified delegate type Callback(T1, T2, T3)
    /// </summary>
    public IEnumerable<Action<T1, T2, T3>> GetCallbacks<T1, T2, T3>()
    {
        foreach (Delegate callback in callbacks.Values)
        {
            if (callback is Action<T1, T2, T3> typedCallback)
            {
                yield return typedCallback;
            }
        }
    }
}

public class Events
{
    /// <summary>
    /// Event dictionary
    /// <param> event name -> event callbacks </param>
    /// </summary>
    private Dictionary<string, EventCallbacks> events = new Dictionary<string, EventCallbacks>();

    /// <summary>
    /// Number of events managed by the event manager
    /// </summary>
    public int Count => events.Count;

    /// <summary>
    /// Add event callback to the event dictionary of a specific event
    /// </summary>
    /// <param name="eventName"> name of the event </param>
    /// <param name="callbackName"> name of the callbcak (used as key) </param>
    /// <param name="callback"> callback delegate </param>
    public void AddEventCallback(string eventName, string callbackName, Delegate callback)
    {
        if (!events.ContainsKey(eventName))
        {
            events[eventName] = new EventCallbacks();
        }
        events[eventName].AddCallback(callbackName, callback);
    }

    /// <summary>
    /// Remove event callback from the event dictionary of a specific event
    /// <param> If the event has no more callbacks, remove the event from the event dictionary </param>
    /// </summary>
    /// <param name="eventName"> name of the event </param>
    /// <param name="callbackName"> name of the callbcak (used as key) </param>
    public void RemoveEventCallback(string eventName, string callbackName)
    {
        if (events.TryGetValue(eventName, out EventCallbacks? eventCallbacks))
        {
            if (eventCallbacks != null)
            {
                eventCallbacks.RemoveCallback(callbackName);
                if (eventCallbacks.Count == 0)
                {
                    events.Remove(eventName);
                }
            }
        }
    }

    /// <summary>
    /// Trigger event with no args
    /// </summary>
    /// <param name="eventName"> name of the event </param>
    public void TriggerEventCallbacks(string eventName)
    {
        if (events.TryGetValue(eventName, out EventCallbacks? eventCallbacks))
        {
            if (eventCallbacks != null)
            {
                foreach (Action callback in eventCallbacks.GetCallbacks())
                {
                    callback();
                }
            }
        }
    }

    /// <summary>
    /// Trigger event with 1 arg
    /// </summary>
    /// <param name="eventName"> name of the event </param>
    /// <param name="t1"> arg1 </param>
    public void TriggerEventCallbacks<T1>(string eventName, T1 t1)
    {
        if (events.TryGetValue(eventName, out EventCallbacks? eventCallbacks))
        {
            if (eventCallbacks != null)
            {
                foreach (Action<T1> callback in eventCallbacks.GetCallbacks<T1>())
                {
                    callback(t1);
                }
            }
        }
    }

    /// <summary>
    /// Trigger event with 2 args
    /// </summary>
    /// <param name="eventName"> name of the event </param>
    /// <param name="t1"> arg1 </param>
    /// <param name="t2"> arg2 </param>
    public void TriggerEventCallbacks<T1, T2>(string eventName, T1 t1, T2 t2)
    {
        if (events.TryGetValue(eventName, out EventCallbacks? eventCallbacks))
        {
            if (eventCallbacks != null)
            {
                foreach (Action<T1, T2> callback in eventCallbacks.GetCallbacks<T1, T2>())
                {
                    callback(t1, t2);
                }
            }
        }
    }

    /// <summary>
    /// Trigger event with 3 args
    /// </summary>
    /// <param name="eventName"> name of the event </param>
    /// <param name="t1"> arg1 </param>
    /// <param name="t2"> arg2 </param>
    /// <param name="t3"> arg3 </param>
    public void TriggerEventCallbacks<T1, T2, T3>(string eventName, T1 t1, T2 t2, T3 t3)
    {
        if (events.TryGetValue(eventName, out EventCallbacks? eventCallbacks))
        {
            if (eventCallbacks != null)
            {
                foreach (Action<T1, T2, T3> callback in eventCallbacks.GetCallbacks<T1, T2, T3>())
                {
                    callback(t1, t2, t3);
                }
            }
        }
    }
}

public class ObjectEvents
{
    /// <summary>
    /// Every object owns its own events
    /// </summary>
    private Dictionary<string, Events> objectEvents = new Dictionary<string, Events>();

    /// <summary>
    /// Add event callback to events relative to an object
    /// </summary>
    /// <param name="objectKey"> exclusive key of object </param>
    /// <param name="eventName"> name of the event </param>
    /// <param name="callbackName"> name of the callbcak (used as key) </param>
    /// <param name="callback"> callback delegate </param>
    public void AddEventCallback(string objectKey, string eventName, string callbackName, Delegate handler)
    {
        if (!objectEvents.ContainsKey(objectKey)) objectEvents[objectKey] = new Events();
        objectEvents[objectKey].AddEventCallback(eventName, callbackName, handler);
    }

    /// <summary>
    /// Remove event callback from events relative to an object
    /// </summary>
    /// <param name="objectKey"> exclusive key of object </param>
    /// <param name="eventName"> name of the event </param>
    /// <param name="callbackName"> name of the callbcak (used as key) </param>
    public void RemoveEventCallback(string objectKey, string eventName, string callbackName)
    {
        if (objectEvents.TryGetValue(objectKey, out Events? events))
        {
            if (events != null)
            {
                events.RemoveEventCallback(eventName, callbackName);
                if (events.Count == 0)
                {
                    objectEvents.Remove(objectKey);
                }
            }
        }
    }

    /// <summary>
    /// Trigger event with no args
    /// </summary>
    /// <param name="objectKey"> exclusive key of object </param>
    /// <param name="eventName"> name of the event </param>
    public void TriggerEventCallbacks(string objectKey, string eventName)
    {
        if (objectEvents.TryGetValue(objectKey, out Events? events))
        {
            if (events != null)
            {
                events.TriggerEventCallbacks(eventName);
            }
        }
    }

    /// <summary>
    /// Trigger event with one arg
    /// </summary>
    /// <param name="objectKey"> exclusive key of object </param>
    /// <param name="eventName"> name of the event </param>
    /// <param name="t1"> arg1 </param>
    public void TriggerEventCallbacks<T1>(string objectKey, string eventName, T1 t1)
    {
        if (objectEvents.TryGetValue(objectKey, out Events? events))
        {
            if (events != null)
            {
                events.TriggerEventCallbacks(eventName, t1);
            }
        }
    }

    /// <summary>
    /// Trigger event with two arg
    /// </summary>
    /// <param name="objectKey"> exclusive key of object </param>
    /// <param name="eventName"> name of the event </param>
    /// <param name="t1"> arg1 </param>
    /// <param name="t2"> arg2 </param>
    public void TriggerEventCallbacks<T1, T2>(string objectKey, string eventName, T1 t1, T2 t2)
    {
        if (objectEvents.TryGetValue(objectKey, out Events? events))
        {
            if (events != null)
            {
                events.TriggerEventCallbacks(eventName, t1, t2);
            }
        }
    }

    /// <summary>
    /// Trigger event with three arg
    /// </summary>
    /// <param name="objectKey"> exclusive key of object </param>
    /// <param name="eventName"> name of the event </param>
    /// <param name="t1"> arg1 </param>
    /// <param name="t2"> arg2 </param>
    /// <param name="t3"> arg3 </param>
    public void TriggerEventCallbacks<T1, T2, T3>(string objectKey, string eventName, T1 t1, T2 t2, T3 t3)
    {
        if (objectEvents.TryGetValue(objectKey, out Events? events))
        {
            if (events != null)
            {
                events.TriggerEventCallbacks(eventName, t1, t2, t3);
            }
        }
    }
}

public class EventManagerCommon : Manager
{

    #region REGION_GLOBAL_EVENT

    private Events globalEvents = new Events();

    #region REGION_GLOBAL_EVENT_REGISTERATION

    private void DoRegisterEntityEvent(string eventName, string callbackName, Delegate handler)
    {
        globalEvents.AddEventCallback(eventName, callbackName, handler);
    }

    public void RegisterGlobalEvent(string eventName, string callbackName, Action callback)
    {
        DoRegisterEntityEvent(eventName, callbackName, callback);
    }
    public void RegisterGlobalEvent<T1>(string eventName, string callbackName, Action<T1> callback)
    {
        DoRegisterEntityEvent(eventName, callbackName, callback);
    }
    public void RegisterGlobalEvent<T1, T2>(string eventName, string callbackName, Action<T1, T2> callback)
    {
        DoRegisterEntityEvent(eventName, callbackName, callback);
    }
    public void RegisterGlobalEvent<T1, T2, T3>(string eventName, string callbackName, Action<T1, T2, T3> callback)
    {
        DoRegisterEntityEvent(eventName, callbackName, callback);
    }

    public void UnregisterGlobalEvent(string eventName, string callbackName)
    {
        globalEvents.RemoveEventCallback(eventName, callbackName);
    }

    #endregion

    #region REGION_GLOBAL_EVENT_TRIGGER

    public void TriggerGlobalEvent(string eventName)
    {
        globalEvents.TriggerEventCallbacks(eventName);
    }
    public void TriggerGlobalEvent<T1>(string eventName, T1 t1)
    {
        globalEvents.TriggerEventCallbacks(eventName, t1);
    }
    public void TriggerGlobalEvent<T1, T2>(string eventName, T1 t1, T2 t2)
    {
        globalEvents.TriggerEventCallbacks(eventName, t1, t2);
    }
    public void TriggerGlobalEvent<T1, T2, T3>(string eventName, T1 t1, T2 t2, T3 t3)
    {
        globalEvents.TriggerEventCallbacks(eventName, t1, t2, t3);
    }

    #endregion

    #endregion

    #region REGION_ENTITY_EVENT

    private ObjectEvents entityEvents = new ObjectEvents();

    #region REGION_ENTITY_EVENT_REGISTRATION

    private void DoRegisterEntityEvent(string eid, string eventName, string callbackName, Delegate handler)
    {
        entityEvents.AddEventCallback(eid, eventName, callbackName, handler);
    }

    public void RegisterEntityEvent(string eid, string eventName, string callbackName, Action callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }
    public void RegisterEntityEvent<T1>(string eid, string eventName, string callbackName, Action<T1> callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }
    public void RegisterEntityEvent<T1, T2>(string eid, string eventName, string callbackName, Action<T1, T2> callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }
    public void RegisterEntityEvent<T1, T2, T3>(string eid, string eventName, string callbackName, Action<T1, T2, T3> callback)
    {
        DoRegisterEntityEvent(eid, eventName, callbackName, callback);
    }

    public void UnregisterEntityEvent(string eid, string eventName, string callbackName)
    {
        entityEvents.RemoveEventCallback(eid, eventName, callbackName);
    }

    #endregion

    #region REGION_ENTITY_EVENT_TRIGGER

    public void TriggerEntityEvent(string eid, string eventName)
    {
        entityEvents.TriggerEventCallbacks(eid, eventName);
    }
    public void TriggerEntityEvent<T1>(string eid, string eventName, T1 t1)
    {
        entityEvents.TriggerEventCallbacks(eid, eventName, t1);
    }
    public void TriggerEntityEvent<T1, T2>(string eid, string eventName, T1 t1, T2 t2)
    {
        entityEvents.TriggerEventCallbacks(eid, eventName, t1, t2);
    }
    public void TriggerEntityEvent<T1, T2, T3>(string eid, string eventName, T1 t1, T2 t2, T3 t3)
    {
        entityEvents.TriggerEventCallbacks(eid, eventName, t1, t2, t3);
    }

    #endregion

    #endregion

    #region REGION_PROXY_EVENT

    private ObjectEvents proxyEvents = new ObjectEvents();

    #region REGION_PROXY_EVENT_REGISTRATION

    private void DoRegisterProxyEvent(string pid, string eventName, string callbackName, Delegate handler)
    {
        proxyEvents.AddEventCallback(pid, eventName, callbackName, handler);
    }

    public void RegisterProxyEvent(string pid, string eventName, string callbackName, Action callback)
    {
        DoRegisterProxyEvent(pid, eventName, callbackName, callback);
    }
    public void RegisterProxyEvent<T1>(string pid, string eventName, string callbackName, Action<T1> callback)
    {
        DoRegisterProxyEvent(pid, eventName, callbackName, callback);
    }
    public void RegisterProxyEvent<T1, T2>(string pid, string eventName, string callbackName, Action<T1, T2> callback)
    {
        DoRegisterProxyEvent(pid, eventName, callbackName, callback);
    }
    public void RegisterProxyEvent<T1, T2, T3>(string pid, string eventName, string callbackName, Action<T1, T2, T3> callback)
    {
        DoRegisterProxyEvent(pid, eventName, callbackName, callback);
    }

    public void UnregisterProxyEvent(string pid, string eventName, string callbackName)
    {
        proxyEvents.RemoveEventCallback(pid, eventName, callbackName);
    }

    #endregion

    #region REGION_PROXY_EVENT_TRIGGER

    public void TriggerProxyEvent(string pid, string eventName)
    {
        proxyEvents.TriggerEventCallbacks(pid, eventName);
    }
    public void TriggerProxyEvent<T1>(string pid, string eventName, T1 t1)
    {
        proxyEvents.TriggerEventCallbacks(pid, eventName, t1);
    }
    public void TriggerProxyEvent<T1, T2>(string pid, string eventName, T1 t1, T2 t2)
    {
        proxyEvents.TriggerEventCallbacks(pid, eventName, t1, t2);
    }
    public void TriggerProxyEvent<T1, T2, T3>(string pid, string eventName, T1 t1, T2 t2, T3 t3)
    {
        proxyEvents.TriggerEventCallbacks(pid, eventName, t1, t2, t3);
    }

    #endregion

    #endregion

}
