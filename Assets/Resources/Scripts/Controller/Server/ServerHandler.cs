using Mirror;
using PolyPerfect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerHandler
{
    public static GameManager GM = GameManager.GetGM();
    public static void ClientLogin(NetworkConnection conn, VirtualRequest msg)
    {
        var mainRig = GM.MainRig;
        var parent = GameObject.Find("People/Players");
        var spawnedInstance = AvatarManager.Instance.SpawnPlayerFromAvatar(mainRig, msg.message, parent.transform);

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, spawnedInstance);
    }

    public static void ClientTakeOver(NetworkConnection conn, VirtualRequest msg)
    {
        var player = PlayerPoolManager.Instance.GetPlayer(msg.networkId);
        player.takeOver = msg.takeOver;
        player.playerController.takeOver = msg.takeOver;
        player.playerController._moveData = msg.moveData;

        // sync animation to all  proxy
       /* var data = new VirtualResponse
        {
            messageId = ClientMsgType.SyncAnimation,
            moveData = msg.moveData
        };
        NetworkServer.SendToReady(data);*/
    }

    public static void SpawnAIs(NetworkConnection conn, VirtualRequest msg)
    {
        try
        {
            var mainRig = GM.MainRig;
            var spawnAmount = GM.spawnAmount;
            var spawnRadius = GM.spawnRadius;
            var _randomCharacterPlacerScript = GameObject.Find("People").GetComponent<RandomCharacterPlacer>();
            var parent = GameObject.Find("People/AIs");

            var characters = DataManager.Instance.NpcPrefabsList.ToArray();
            var _instances = ((RandomCharacterPlacer)_randomCharacterPlacerScript).SpawnAnimals(mainRig, characters, parent, spawnAmount, spawnRadius);

            GM.AINum += _instances.Length;
            GM.PlayerNum = PlayerPoolManager.Instance.CountPlayer();

            if (GM.MirrorManager.IsActive())
            {
                // to update client rig 
                for (int i = 0; i < _instances.Length; i++)
                {
                    var _fullname = _instances[i].name;
                    var _avatarname = _fullname.Split('_')[0];
                    var _avatarid = _fullname.Split('_')[1];
                    var _avatar = new Avatar()
                    {
                        id = ToolsManager.ParseInt32(_avatarid),
                        type = CHARACTER.AI, // ai
                        aname = _avatarname,
                        animatorController = _instances[i].GetComponent<Animator>().runtimeAnimatorController.name,
                        postion = _instances[i].transform.position,
                        rotation = _instances[i].transform.rotation,
                        scale = _instances[i].transform.localScale
                    };

                    var aiAvatar = _instances[i].GetComponent<VirtualAvatarPlayer>();
                    aiAvatar.avatar = _avatar;

                    // server spawn the instance
                    NetworkServer.Spawn(_instances[i]);
                }

                GM.PlayerNum = PlayerPoolManager.Instance.CountPlayer();
                GM.UpdateUI(GM.PlayerNum, GM.AINum);
 
            }
        }
        finally
        {
            // to update client ccu ui
            var data = new VirtualResponse
            {
                messageId = ClientMsgType.UpdateUI,
                uiData = new UIData
                {
                    playerNum = GM.PlayerNum,
                    aiNum = GM.AINum
                }
            };
            NetworkServer.SendToReady(data);
        }
    }
}
