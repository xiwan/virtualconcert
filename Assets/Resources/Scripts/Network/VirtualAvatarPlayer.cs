using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirtualAvatarPlayer : NetworkBehaviour
{
    [SyncVar]
    public Avatar avatar;


    private void Awake()
    {
        
    }

    private void Start()
    {
        if (isClient)
        {
            if (avatar.type == 0)
            {
                var parent = GameObject.Find("People/Players");
                this.transform.SetParent(parent.transform, false);
            }
            if (avatar.type == 1)
            {
                var parent = GameObject.Find("People/AIs");
                this.transform.SetParent(parent.transform, false);
            }
            Debug.Log(this.gameObject.name);
            var spawnedInstance = AvatarManager.SpawnFromAvatar(this.gameObject, avatar);
            Debug.Log(this.gameObject.name);
        }
    }
    public override void OnStartClient()
    {
        
        
    }
}
