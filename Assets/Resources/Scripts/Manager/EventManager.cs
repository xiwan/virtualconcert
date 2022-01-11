using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public enum EVENT 
{
    UISpawnAIs = 0,
    UIPickAny = 1,
    UIClientAuth = 2,
    UIServerAuth = 3,

    CameraFollow = 10,
};

public class EventManager : Single<EventManager>
{
    private Dictionary<EVENT, Action> eventTable = new Dictionary<EVENT, Action>();


    public void LoadEvent()
    {
        this.AddHandler(EVENT.UISpawnAIs, () =>
        {
            if (GameManager.GetVNM().IsActive())
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

        this.AddHandler(EVENT.UIClientAuth, () =>
        {

        });

        this.AddHandler(EVENT.UIServerAuth, () =>
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
