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

    public GameObject MainRig;

    private List<Avatar> npcPrefabsList = new List<Avatar>();

    private void LoadPrefabs()
    {
        var man01 = new Avatar() {
            id = 1,
            aname = "man-astronaut",
            animatorController = "MoonwalkController"
        };
        var man02 = new Avatar()
        {
            id = 2,
            aname = "man-basketball-player",
            animatorController = "BellyController"
        };
        var man03 = new Avatar()
        {
            id = 3,
            aname = "man-boxer",
            animatorController = "BrooklynuprockController"
        };
        var man04 = new Avatar()
        {
            id = 4,
            aname = "man-business",
            animatorController = "FlairController"
        };
        var man05 = new Avatar()
        {
            id = 5,
            aname = "man-casual",
            animatorController = "HouseController"
        };
        var man06 = new Avatar()
        {
            id = 6,
            aname = "man-chef",
            animatorController = "JazzController"
        };
        var man07 = new Avatar()
        {
            id = 7,
            aname = "man-clown",
            animatorController = "MoonwalkController"
        };
        var man08 = new Avatar()
        {
            id = 8,
            aname = "man-construction-worker",
            animatorController = "NorthsoulController"
        };
        var man09 = new Avatar()
        {
            id = 9,
            aname = "man-cowboy",
            animatorController = "SambaController"
        };
        var man10 = new Avatar()
        {
            id = 10,
            aname = "man-cyclist",
            animatorController = "YmcaController"
        };
        npcPrefabsList.Add(man01);
        npcPrefabsList.Add(man02);
        npcPrefabsList.Add(man03);
        npcPrefabsList.Add(man04);
        npcPrefabsList.Add(man05);
        npcPrefabsList.Add(man06);
        npcPrefabsList.Add(man07);
        npcPrefabsList.Add(man08);
        npcPrefabsList.Add(man09);
        npcPrefabsList.Add(man10);
    }

    public Avatar[] Avatars { get; set; }

    [ContextMenu("Spawn Animals")]
    public void SpawnAnimals()
    {
        StartCoroutine(NetworkManagerSpawnAnimals());        
    }

    public static GameManager getGM()
    { 
        return GameObject.Find("GameManager").GetComponent<GameManager>();
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

    void Awake()
    {
        LoadPrefabs();
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
            if (NetworkManager.singleton.isNetworkActive)
            {
                //var characters = NetworkManager.singleton.spawnPrefabs.ToArray();
                var _instances = ((RandomCharacterPlacer)_randomCharacterPlacerScript)
                    .SpawnAnimals(MainRig, npcPrefabsList.ToArray(), parent, spawnAmount, spawnRadius);

                var _avatarList = new List<Avatar>();
                for (int i = 0; i < _instances.Length; i++)
                {
                    var _fullname = _instances[i].name;
                    var _avatarname = _fullname.Split('_')[0];
                    var _avatarid = _fullname.Split('_')[1];
                    var _avatar = new Avatar()
                    {
                        id = ToolsManager.ParseInt32(_avatarid),
                        type = 1, // ai
                        aname = _avatarname,
                        animatorController = _instances[i].GetComponent<Animator>().runtimeAnimatorController.name,
                        postion = _instances[i].transform.position,
                        rotation = _instances[i].transform.rotation,
                        scale = _instances[i].transform.localScale
                    };

                    var aiAvatar = _instances[i].GetComponent<VirtualAvatarPlayer>();
                    aiAvatar.avatar = _avatar;

                    // server spawn the instance
                    NetworkServer.Spawn(_instances[i]);
                    //_avatarList.Add(_avatar);
                }
                

                //VirtualResponse msg = new VirtualResponse
                //{
                //    messageId = 0x0002,
                //    message = new VirtualAvatarCreateMessage
                //    {
                //        avatars = _avatarList.ToArray()
                //    }
                //};
                //NetworkServer.SendToAll(msg);               
            } 
        }
        else
        {
            ((RandomCharacterPlacer)_randomCharacterPlacerScript).SpawnAnimals(parent, spawnAmount, spawnRadius);
        }
    }



}
