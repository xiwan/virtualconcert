using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct VirtualRequest : NetworkMessage
{
    public ServerMsgType messageId;
    public int networkId;
    public bool takeOver;
    public string content;
    public byte[] payload;
    public Avatar message;
    public MoveData moveData;
}

public struct VirtualResponse : NetworkMessage
{
    public ClientMsgType messageId;
    public string content;
    public int playerNum;
    public int aiNum;
    public byte[] payload;
}

public struct VirtualAvatarCreateMessage 
{
    public Avatar[] avatars;
}

public class MoveData
{
    public float horizontal = 0;
    public float vertical = 0;
    public float speed = 0;

    public bool idle = false;
    public bool walk = false;
    public bool jump = false;
    public bool dance = false;
    public bool sprint = false;
}
