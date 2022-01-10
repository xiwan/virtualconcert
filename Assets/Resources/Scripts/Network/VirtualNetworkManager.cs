using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtualNetworkManager : NetworkManager
{
    public GameObject MainRig;

    CameraChange _cameraChangeScript;
    private Transform _groundCheck;


    // only called on client
    NetworkBehaviour GetConnPlayer()
    {
        var playerAvatar = NetworkClient.connection.identity.gameObject.GetComponent<VirtualAvatarPlayer>();
        if (playerAvatar == null)
        {
            throw new Exception("not found alive conn");
        }
        return playerAvatar;
    }

    // only called on client
    uint GetNetId()
    {
        return NetworkClient.connection.identity.netId;
    }

    public void CommandOnServer(EVENT evt)
    {
        if (evt == EVENT.UISpawnAIs)
        {
            (GetConnPlayer() as VirtualAvatarPlayer).SpawnAIsOnServer();
        }
        else if(evt == EVENT.UIPickAny)
        {
            //(GetConnPlayer() as VirtualAvatarPlayer).PickAnyAIOnServer();
        }

    }

    public override void Awake()
    {
        base.Awake();

    }

    public override void Start()
    {
        base.Start();
        //_cameraChangeScript = GameObject.Find("CameraGroups").GetComponent<CameraChange>();
    }

    private void Update()
    {
        //CameraFollow(true);
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
        NetworkClient.RegisterPrefab(MainRig);
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

    private void CameraFollow(bool flag)
    {
        var follower = GameObject.Find("Follower");
        var target = GetConnPlayer();

        _cameraChangeScript.CameraFollow(follower.transform, target.transform, new Vector3(0, 1.8f, 0));
        _cameraChangeScript.CameraSwitch(flag);
    }


}
