using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Mirror;

public class VirutalSpawnNetworkManager : MonoBehaviour
{
    public GameObject mainRigAll;


    private void Start()
    {
        //ServerListen();
    }

    // Register prefab and connect to the server  
    public void ClientConnect()
    {
        NetworkClient.RegisterPrefab(mainRigAll);
        NetworkClient.RegisterHandler<RpcMessage>(OnClientConnect);

        NetworkClient.Connect("localhost");
    }

    void OnClientConnect(RpcMessage msg)
    {
        Debug.Log(msg);
        Debug.Log("Connected to server.");
    }

    public void ServerListen()
    {
        NetworkServer.RegisterHandler<RpcMessage>(OnServerConnect);
        NetworkServer.RegisterHandler<ReadyMessage>(OnClientReady);

        // start listening, and allow up to 4 connections
        NetworkServer.Listen(4);
    }

    void SpawnTrees()
    {
        Debug.Log("Spawn Trees");
    }

    void OnClientReady(NetworkConnection conn, ReadyMessage msg)
    {
        Debug.Log("Client is ready to start: " + conn);
        NetworkServer.SetClientReady(conn);
        SpawnTrees();
    }

    void OnServerConnect(NetworkConnection conn, RpcMessage msg)
    {
        Debug.Log("New client connected: " + conn);
    }

}
