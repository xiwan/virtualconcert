using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum EVENT 
{
    TakeOverEventOn,
    TakeOverEventOff,
};
public class EventManager : Single<EventManager>
{
    private Dictionary<EVENT, Action> eventTable = new Dictionary<EVENT, Action>();

    
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
        this.AddHandler(evt, action);
    }

    public void Broadcast (EVENT evt)
    {
        this.Trigger(evt);
    }

}
