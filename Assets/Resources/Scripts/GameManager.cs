using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PolyPerfect;

public class GameManager : MonoBehaviour
{

    MonoBehaviour _cameraChangeScript;
    MonoBehaviour _randomCharacterPlacerScript;

    Text _ccuTex;

    public int selectedPlayer = 0;

    public int lastSelectedPlayer = 0;

    public bool resetCamera = false;

    public bool pickAnyPlayer = false;

    public float spawnRadius = 20;
    public int spawnAmount = 20;

    public Transform NetworkAttach;

    public Dictionary<int, int> selectedPlayerDict = new Dictionary<int, int>();

    [ContextMenu("Spawn Animals")]
    public void SpawnAnimals()
    {
        StartCoroutine(NetworkManagerSpawnAnimals());        
    }

    public void SelectPlayer(int playerId)
    {
        lastSelectedPlayer = selectedPlayer;
        selectedPlayer = playerId;
        if (lastSelectedPlayer != selectedPlayer)
        {
            PlayerPool.GetInstance().ResetDataExcept(selectedPlayer);
        }
    }

    public Player PickAnyPlayer()
    {
        var player = PlayerPool.GetInstance().GetAnyPlayer();
        if (player != null)
        {
            player.MoveController.takeOver = true;
            SelectPlayer(player.InstanceId);
        }
        return player;
    }

    public void DeselectPlayer(int playerId)
    {
        if (playerId == 0)
        {
            selectedPlayer = playerId;
            lastSelectedPlayer = selectedPlayer;
            PlayerPool.GetInstance().ResetDataExcept(playerId);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _cameraChangeScript = GameObject.Find("CameraGroups").GetComponent<CameraChange>();       
        _randomCharacterPlacerScript = GameObject.Find("People").GetComponent<RandomCharacterPlacer>();
        _ccuTex = GameObject.Find("Counter").GetComponent<Text>();

        StartCoroutine(UpdateUITask());
    }

    // Update is called once per frame
    void Update()
    {
        SelectPlayerTask();

        //UpdateUITask();
    }

    private void SelectPlayerTask()
    {
        if (resetCamera)
        {
            pickAnyPlayer = false;
            selectedPlayer = 0;
        }

        if (pickAnyPlayer)
        {
            resetCamera = false;
            if (selectedPlayer == 0)
            {
                selectedPlayer = PickAnyPlayer().InstanceId;
            }
        }

        if (selectedPlayer == 0)
        {
            DeselectPlayer(0);
            ((CameraChange)_cameraChangeScript).CameraSwitch(false);
        }
        else
        {
            var player = PlayerPool.GetInstance().GetPlayer(selectedPlayer);
            if (player != null)
            {
                var follower = player.Follower;
                var target = player.MoveController;
                ((CameraChange)_cameraChangeScript).CameraFollow(follower.transform, target.transform, new Vector3(0, 1.8f, 0));
                ((CameraChange)_cameraChangeScript).CameraSwitch(true);
            }
             
        }
    }

    IEnumerator UpdateUITask()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            _ccuTex.text = "CCU: " + PlayerPool.GetInstance().CountPlayer();
        }
    }

    IEnumerator NetworkManagerSpawnAnimals()
    {
        yield return new WaitForSeconds((Random.Range(0, 200) / 100));
        var parent = GameObject.Find("People/AIs");

        if (NetworkManager.singleton != null)
        {
            if (NetworkManager.singleton.isNetworkActive && NetworkAttach != null)
            {
                var characters = NetworkManager.singleton.spawnPrefabs.ToArray();
                var instances = ((RandomCharacterPlacer)_randomCharacterPlacerScript).SpawnAnimals(parent, spawnAmount, spawnRadius);

                //for (int i = 0; i < instances.Length; i++)
                //{

                //    //instances[i].AddComponent<NetworkIdentity>();
                //    //instances[i].AddComponent<NetworkTransform>();
                //    //var na = instances[i].AddComponent<NetworkAnimator>();
                //    //na.animator = instances[i].GetComponent<Animator>();
                //    NetworkServer.Spawn(instances[i]);
                //}

                for (int i = 0; i < instances.Length; i++)
                {
                    var networkAttach = Instantiate(NetworkAttach, instances[i].transform, false);
                    networkAttach.name = NetworkAttach.name + networkAttach.GetInstanceID();
                    networkAttach.GetComponent<NetworkAnimator>().animator = instances[i].GetComponent<Animator>();
                    //var networkAttachPath = "/" + instances[i].name + "/" + networkAttach.name;
                    NetworkServer.Spawn(networkAttach.gameObject);
                }
            } 
        }
        else
        {
            ((RandomCharacterPlacer)_randomCharacterPlacerScript).SpawnAnimals(parent, spawnAmount, spawnRadius);
        }
    }



}
