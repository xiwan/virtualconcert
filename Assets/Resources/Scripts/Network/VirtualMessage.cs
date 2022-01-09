using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct VirtualRequest : NetworkMessage
{
    public int messageId;
    public int networkId;
    public bool takeOver;
    public string content;
    public byte[] payload;
    public Avatar message;
    public MoveData moveData;
}

public struct VirtualResponse : NetworkMessage
{
    public int messageId;
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
    public float horizontal;
    public float vertical;
    public Vector3 hmove;
    public Vector3 vmove;
    public float angle;
}
