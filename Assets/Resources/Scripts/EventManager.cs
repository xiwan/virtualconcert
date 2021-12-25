using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum EVENT 
{
    TakeOverEventOn,
    TakeOverEventOff,
};
public class EventManager 
{
    private Dictionary<EVENT, Action> eventTable = new Dictionary<EVENT, Action>();

    private static EventManager _instance;

    public static EventManager getInstance()
    {
        if (_instance == null)
        {
            _instance = new EventManager();
        }
        return _instance;
    }
    
    public void AddHandler(EVENT evt, Action action)
    {
        if (!eventTable.ContainsKey(evt))
        {
            eventTable[evt] = action;
        }
        else
        {
            eventTable[evt] += action;
        }
    }

    public void Trigger (EVENT evt)
    {
        eventTable[evt]?.Invoke();
    }

    public void AddGlobalHandler(EVENT evt, Action action)
    {
        getInstance().AddHandler(evt, action);
    }

    public void Broadcast (EVENT evt)
    {
        getInstance().Trigger(evt);
    }

}
