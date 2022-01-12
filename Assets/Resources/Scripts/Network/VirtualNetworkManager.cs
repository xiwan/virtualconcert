using Mirror;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VirtualNetworkManager : NetworkManager
{

    public void UpdateUI()
    {
        var PlayerNum = GameObject.Find("People/Players").transform.childCount;
        var AINum = GameObject.Find("People/AIs").transform.childCount;
        var _ccuTex = GameObject.Find("Counter").GetComponent<Text>();
        _ccuTex.text = "Player: " + PlayerNum + " AI:" + AINum;
    }

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

    void Update()
    {
        UpdateUI();
    }

    IEnumerator BroadCastMsgToAll()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            do
            {
                if (broadcastMsg.TryDequeue(out VirtualResponse msg))
                {
                    var moveData = msg.moveData;
                    //Debug.Log(moveData.walk + " x " + moveData.speed + " x " + moveData.dance + " x " + moveData.jump + " x " + moveData.networkId + " length:" + broadcastMsg.Count);
                    //Debug.Log(_isWalking + "=" + _isDancing + "=" + _isJumping + "=");
                    // send to all ready clients
                    NetworkServer.SendToReady(msg);
                }
            }
            while (broadcastMsg.Count > 0);
        }
    }

    private static ConcurrentQueue<VirtualResponse> broadcastMsg = new ConcurrentQueue<VirtualResponse>();
    public void PushBroadMsg(VirtualResponse data)
    {
        broadcastMsg.Enqueue(data);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<VirtualRequest>(ServerRouteTable.Instance.ReceiveMsg);

        //StartCoroutine(BroadCastMsgToAll());
        
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

    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkClient.RegisterHandler<VirtualResponse>(ClientRouteTable.Instance.ReceiveMsg);
        NetworkClient.RegisterPrefab(GameManager.GetGM().MainRig);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("Connected to server: ");

        var playersPrefabs = DataManager.Instance.PlayerRrefabsList.ToArray();
        var value = UnityEngine.Random.Range(0, playersPrefabs.Length);
        var selectedName = playersPrefabs[value];

        // need serilizer later
        VirtualRequest msg = new VirtualRequest
        {
            messageId = ServerMsgType.ClientLogin,
            content = selectedName,
            message = new Avatar
            {
                id = 101,
                type = CHARACTER.Player, // player
                aname = selectedName,
                animatorController = "UserController"
            }
        };

        NetworkClient.connection.Send(msg);
    }

    public override void OnClientDisconnect()
    {
       

        // need serilizer later
        VirtualRequest msg = new VirtualRequest
        {
            messageId = ServerMsgType.ClientLogout,
            networkId = Convert.ToInt32(GetNetId()),
        };

        NetworkClient.connection.Send(msg);

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
