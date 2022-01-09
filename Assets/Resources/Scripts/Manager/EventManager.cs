using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum EVENT 
{
    UISpawnAIs,
    UIPickAny,

};

public class EventManager : Single<EventManager>
{
    private Dictionary<EVENT, Action> eventTable = new Dictionary<EVENT, Action>();


    public void LoadEvent()
    {
        this.AddHandler(EVENT.UISpawnAIs, () =>
        {
            if (GameManager.GetGM().IsMirror())
            {
                GameManager.GetVNM().CommandOnServer(EVENT.UISpawnAIs);
            }
            else
            {
                GameManager.GetGM().SpawnAnimals();
            }
        });

        this.AddHandler(EVENT.UIPickAny, () =>
        {

        });
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
        this.AddHandler(evt, action);
    }

    public void Broadcast (EVENT evt)
    {
        this.Trigger(evt);
    }

}
