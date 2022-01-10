using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PolyPerfect;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public float spawnRadius = 20;

    public int spawnAmount = 20;

    public NetworkManager MirrorManager;

    public GameObject MainRig;

    private MonoBehaviour _randomCharacterPlacerScript;

    public int PlayerNum;

    public int AINum;

    public static GameManager GetGM()
    {
        return GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public static VirtualNetworkManager GetVNM()
    {
        return GameObject.Find("VirtualNetworkManager").GetComponent<VirtualNetworkManager>();
    }


    public void Initialize()
    {
        // initialization goes here
        DataManager.Initialize();
        AvatarManager.Initialize();
        PlayerPoolManager.Initialize();
        EventManager.Initialize();

        ServerRouteTable.Initialize();
        ClientRouteTable.Initialize();
    }

    public void LoadData()
    {
        
        // data load goes here
        DataManager.Instance.LoadPrefabsData();
        EventManager.Instance.LoadEvent();

        this.LoadGameObjects();

        ServerRouteTable.Instance.RegisterHandlers();
        ClientRouteTable.Instance.RegisterHandlers();
    }

    public void CleanData()
    {
        PlayerPoolManager.Instance.ResetDataExcept(0);
    }

    [ContextMenu("Spawn Animals")]
    public void SpawnAnimals()
    {
        StartCoroutine(NetworkManagerSpawnAnimals());        
    }

    public bool IsMirror()
    {
        return NetworkManager.singleton != null && NetworkManager.singleton.isNetworkActive;
    }

    public void LoadGameObjects()
    {
        if (MirrorManager == null)
            MirrorManager = GetVNM();
        if (MainRig == null)
            MainRig = (GameObject)Resources.Load("Prefabs/Network/MainRigAll");
        if (_randomCharacterPlacerScript == null)
            _randomCharacterPlacerScript = GameObject.Find("People").GetComponent<RandomCharacterPlacer>();
    }

    void Awake()
    {
        Initialize();
        LoadData();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //SelectPlayerTask();

        //UpdateUITask();

    }

    public void  UpdateUI(int playerNum, int aiNum)
    {
        var _ccuTex = GameObject.Find("Counter").GetComponent<Text>();
        _ccuTex.text = "Player: " + playerNum + " AI:" + aiNum;
    }

    IEnumerator NetworkManagerSpawnAnimals()
    {
        yield return new WaitForSeconds((Random.Range(0, 200) / 100));

        try 
        {
            var parent = GameObject.Find("People/AIs");
            var characters = DataManager.Instance.NpcPrefabsList.ToArray();
            var _instances = ((RandomCharacterPlacer)_randomCharacterPlacerScript).SpawnAnimals(MainRig, characters, parent, spawnAmount, spawnRadius);
            AINum += _instances.Length;

            if (this.IsMirror())
            {
                // to update client rig 
                for (int i = 0; i < _instances.Length; i++)
                {
                    var _fullname = _instances[i].name;
                    var _avatarname = _fullname.Split('_')[0];
                    var _avatarid = _fullname.Split('_')[1];
                    var _avatar = new Avatar()
                    {
                        id = ToolsManager.ParseInt32(_avatarid),
                        type = CHARACTER.AI, // ai
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
                }

                // to update client ccu ui
                var rsp = new VirtualResponse
                {
                    messageId = ClientMsgType.UpdateUI,
                    playerNum = PlayerPoolManager.Instance.CountPlayer(),
                    aiNum = AINum
                };

                NetworkServer.SendToReady(rsp);
            }
        }
        finally
        {
            PlayerNum = PlayerPoolManager.Instance.CountPlayer();
            UpdateUI(PlayerNum, AINum);
        }  
        
    }



}
