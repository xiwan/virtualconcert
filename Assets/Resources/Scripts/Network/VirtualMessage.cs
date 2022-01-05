using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct VirtualRequest : NetworkMessage
{
    public int messageId;
    public string content;
    public byte[] payload;
    public Avatar message;
}

public struct VirtualResponse : NetworkMessage
{
    public int messageId;
    public string content;
    public byte[] payload;
    public VirtualAvatarCreateMessage message;
}

public struct VirtualAvatarCreateMessage 
{
    public Avatar[] avatars;
}
