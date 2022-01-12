using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class ClientHandler
{
    public static GameManager GM = GameManager.GetGM();

    public static void UpdateUI(VirtualResponse msg)
    {
        GM.UpdateUI(msg.uiData.playerNum, msg.uiData.aiNum);
    }

    public static void SyncAnimation(VirtualResponse msg)
    {
        var player = GM.MirrorManager.GetConnPlayer();
        Debug.Log(player.name + " " + GM.MirrorManager.GetNetId() + " " + msg.moveData);
        if (player != null)
        {
            player.GetComponent<VirtualAvatarPlayer>().MoveAnimation(msg.moveData);
        }
    }


}