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
        
    }

    public static void SyncAnimation(VirtualResponse msg)
    {
        var player = PlayerPoolManager.Instance.GetPlayer(msg.moveData.networkId);
        if (player)
        {
            var moveData = msg.moveData;
            //Debug.Log("client: " + moveData.walk + " x " + moveData.speed + " x " + moveData.dance + " x " + moveData.jump + " x " + moveData.networkId);
            player.GetComponent<VirtualAvatarPlayer>().MoveAnimation(msg.moveData);
        }
    }


}