using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PolyPerfect;

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
            var _wanderScript = this.gameObject.GetComponent<WanderScript>();
            if (_wanderScript)
            {
                _wanderScript.enabled = false;
            }
            var spawnedInstance = AvatarManager.Instance.SpawnFromAvatar(this.gameObject, avatar);
            Debug.Log(this.gameObject.name);
        }
    }

    public override void OnStartClient()
    {


    }

    [Command]
    public void SpawnAIs()
    {
        GameManager.GetGM().SpawnAnimals();
    }


}

