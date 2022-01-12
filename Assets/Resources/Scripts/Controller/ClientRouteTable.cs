using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ClientMsgType
{
    UpdateUI = 0x0100,
    SyncAnimation = 0x0101
}
public class ClientRouteTable : Single<ClientRouteTable>
{
    readonly Dictionary<ClientMsgType, Action<VirtualResponse>> Route = new Dictionary<ClientMsgType, Action<VirtualResponse>>();
    public void RegisterHandlers()
    {
        Route.Add(ClientMsgType.UpdateUI, ClientHandler.UpdateUI);
        Route.Add(ClientMsgType.SyncAnimation, ClientHandler.SyncAnimation);

    }
    public void ReceiveMsg(VirtualResponse msg)
    {
        if (Route.ContainsKey(msg.messageId))
        {
            Route[msg.messageId].Invoke(msg);
        }
    }

   
}
