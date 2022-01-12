using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtualNetworkManager : NetworkManager
{

    public bool IsActive()
    {
        return this != null && this.isNetworkActive;
    }

    public bool IsServer()
    {
        return NetworkClient.connection != null && NetworkClient.connection.identity.isServer;
    }

    public bool IsClient()
    {
        return NetworkClient.connection != null && NetworkClient.connection.isReady;
    }

    public bool ServerOwn()
    {
        return NetworkClient.connection != null && NetworkClient.connection.identity.connectionToClient == null;
    }

    public bool ClientOwn()
    {
        return !ServerOwn() && NetworkClient.connection.identity.hasAuthority;
    }

    public bool ProxyOwn()
    {
        return !ServerOwn() && !NetworkClient.connection.identity.hasAuthority;
    }

    public void SetFixedDeltaTimeForServer()
    {
        Time.fixedDeltaTime = 1f / 30;
    }

    public void SetFixedDeltaTimeForClient()
    {
        Time.fixedDeltaTime = 1f / 30;
    }

    // only called on client
    public GameObject GetConnPlayer()
    {
        if (!NetworkClient.connection.isReady) return null;
        if (!NetworkClient.connection.identity) return null;

        var playerAvatar = NetworkClient.connection.identity.gameObject;
        if (playerAvatar == null)
        {
            throw new Exception("not found alive conn");
        }
        return playerAvatar;
    }

    // only called on client
    public uint GetNetId()
    {
        return NetworkClient.connection.identity.netId;
    }

    public void CommandOnServer(EVENT evt)
    {
        if (evt == EVENT.UISpawnAIs)
        {
            GetConnPlayer().GetComponent<VirtualAvatarPlayer>().SpawnAIsOnServer();
        }
    }

    public override void Awake()
    {
        base.Awake();
        if (this.IsClient())
        {
            SetFixedDeltaTimeForClient();
        }
        else
        {
            SetFixedDeltaTimeForServer();
        }

    }

    public override void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<VirtualRequest>(ServerRouteTable.Instance.ReceiveMsg);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        GameManager.GetGM().CleanData();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Debug.Log("======================");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("Connected to server: ");
        // need serilizer later
        VirtualRequest msg = new VirtualRequest
        {
            messageId = ServerMsgType.ClientLogin,
            content = "woman-police",
            message = new Avatar
            {
                id = 101,
                type = CHARACTER.Player, // player
                aname = "woman-police",
                animatorController = "YmcaController"
            }
        };

        NetworkClient.connection.Send(msg);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkClient.RegisterHandler<VirtualResponse>(ClientRouteTable.Instance.ReceiveMsg);
        NetworkClient.RegisterPrefab(GameManager.GetGM().MainRig);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
    }

    public override void OnClientError(Exception exception)
    {
        base.OnClientError(exception);
    }

    public override void OnClientNotReady()
    {
        base.OnClientNotReady();
    }

}
