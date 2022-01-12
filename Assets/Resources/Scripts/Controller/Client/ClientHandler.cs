﻿using Mirror;
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
        foreach (MoveData data in msg.moveDataList)
        {
            var player = PlayerPoolManager.Instance.GetPlayer(data.networkId);
            if (player)
            {
                player.GetComponent<VirtualAvatarPlayer>().MoveAnimation(data);
            }
        }
    }


}