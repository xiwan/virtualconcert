using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public int instanceId;
    public int networkId;
    public string instanceName;
    public bool takeOver;
    public GameObject follower;
    public MoveController moveController;
    public VirtualAvatarPlayer playerController;


}
