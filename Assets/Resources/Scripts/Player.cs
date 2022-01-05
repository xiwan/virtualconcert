using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    int instanceId;
    bool takeOver;
    GameObject follower;

    MoveController moveController;

    public int InstanceId
    {
        get {return instanceId;}
        set {
            instanceId = value;
        }
    }

    public bool TakeOver
    {
        get {return takeOver;}
        set {
            takeOver = value;
        }
    }

    public GameObject Follower
    {
        get {return follower;}
        set {follower = value;}
    }

    public MoveController MoveController
    {
        get { return moveController;}
        set { 
            moveController = value;
            takeOver = value;
        }
    }

}
