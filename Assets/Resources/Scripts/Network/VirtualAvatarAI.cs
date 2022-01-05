using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirtualAvatarAI : NetworkBehaviour
{
    [SyncVar]
    public Avatar avatar;

    private void Awake()
    {
        var parent = GameObject.Find("People/AIs");
        this.transform.SetParent(parent.transform, false);
    }

    private void Start()
    {
        var spawnedInstance = AvatarManager.SpawnFromAvatar(this.gameObject, avatar);
        Debug.Log(spawnedInstance.name);
    }

    public override void OnStartClient()
    {

        
    }

    

}
