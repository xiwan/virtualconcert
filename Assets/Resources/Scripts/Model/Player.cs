using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public int instanceId;
    public bool takeOver;
    public GameObject follower;
    public MoveController moveController;


}
