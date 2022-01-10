using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ClientMsgType
{
    UpdateUI = 0x0100,
}
public class ClientRouteTable : Single<ClientRouteTable>
{
    readonly Dictionary<ClientMsgType, Action<VirtualResponse>> Route = new Dictionary<ClientMsgType, Action<VirtualResponse>>();

    public void ReceiveMsg(VirtualResponse msg)
    {
        if (Route.ContainsKey(msg.messageId))
        {
            Route[msg.messageId].Invoke(msg);
        }
    }

    public void RegisterHandlers()
    {
        Route.Add(ClientMsgType.UpdateUI, ClientHandler.UpdateUI);

    }
}
