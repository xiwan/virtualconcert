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
        GM.UpdateUI(msg.playerNum, msg.aiNum);
    }


}