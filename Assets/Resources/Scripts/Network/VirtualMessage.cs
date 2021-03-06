using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct VirtualRequest : NetworkMessage
{
    public int msgId;
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
    public int msgId;
    public ClientMsgType messageId;
    public string content;
    public byte[] payload;
    public MoveData moveData;
    public UIData uiData;
}

public struct VirtualAvatarCreateMessage 
{
    public Avatar[] avatars;
}

public class UIData
{
    public int playerNum;
    public int aiNum;
}

public class MoveData
{
    public int networkId;

    public float horizontal = 0;
    public float vertical = 0;
    public float speed = 0;

    public bool idle = false;
    public bool walk = false;
    public bool jump = false;
    public bool dance = false;
    public bool sprint = false;
}
