using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum ServerMsgType
{ 
    ClientLogin = 0x0010,
    ClientTakeOver = 0x0011,
    SpawnAIs = 0x0012
}
public class ServerRouteTable : Single<ServerRouteTable>
{
    readonly Dictionary<ServerMsgType, Action<NetworkConnection, VirtualRequest>> Route = new Dictionary<ServerMsgType, Action<NetworkConnection, VirtualRequest>>();

    public void ReceiveMsg(NetworkConnection conn, VirtualRequest msg)
    {
        if (Route.ContainsKey(msg.messageId))
        {
            Route[msg.messageId].Invoke(conn, msg);
        }
    }

    public void RegisterHandlers()
    {
        Route.Add(ServerMsgType.ClientLogin, ServerHandler.ClientLogin);
        Route.Add(ServerMsgType.ClientTakeOver, ServerHandler.ClientTakeOver);
        Route.Add(ServerMsgType.SpawnAIs, ServerHandler.SpawnAIs);
    }

}
