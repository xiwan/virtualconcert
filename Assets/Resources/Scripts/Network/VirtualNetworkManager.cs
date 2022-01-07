using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VirtualNetworkManager : NetworkManager
{
    private GameManager _gm;
    public GameObject MainRig;

    public void CommandOnServer(int clickId)
    {
        var playerAvatar = NetworkClient.connection.identity.gameObject.GetComponent<VirtualAvatarPlayer>();
        if (playerAvatar != null)
        {
            playerAvatar.SpawnAIs();
        }
    }

    public override void Awake()
    {
        base.Awake();
        _gm = GameManager.GetGM(); 

    }

    public override void Start()
    {
        base.Start();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<VirtualRequest>(OnServerReceiveMsg);
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
            messageId = 0x0001,
            content = "woman-police",
            message = new Avatar
            {
                id = 101,
                type = 0, // player
                aname = "woman-police",
                animatorController = "YmcaController"
            }
        };

        NetworkClient.connection.Send(msg);

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkClient.RegisterHandler<VirtualResponse>(OnClientReceiveMsg);
        // generate a new unique assetId 
        System.Guid creatureAssetId = System.Guid.NewGuid();

        //NetworkClient.RegisterSpawnHandler(creatureAssetId, SpawnDelegate, UnSpawnDelegate);
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

    void OnServerReceiveMsg(NetworkConnection conn, VirtualRequest msg)
    {
        var spawnedInstance = AvatarManager.Instance.SpawnPlayerFromAvatar(MainRig, msg.message);

        // call this to use this gameobject as the primary controller
        Debug.Log("called from server");
        NetworkServer.AddPlayerForConnection(conn, spawnedInstance);
        //NetworkServer.Spawn(spawnedInstance);
    }

    void OnClientReceiveMsg(VirtualResponse msg)
    {
        Debug.Log("called from client");
        var parent = GameObject.Find("People/AIs");
        foreach (Avatar ava in msg.message.avatars)
        {
            //Debug.Log(ava.animatorController);
            //var instance = Instantiate(playerPrefab, ava.postion, ava.rotation, parent.transform);
            //var spawnedInstance = AvatarManager.SpawnFromAvatar(instance, ava);
        }
    }
}
