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

    public VirtualNetworkManager MirrorManager;

    public GameObject MainRig;

    private MonoBehaviour _randomCharacterPlacerScript;

    public int PlayerNum;

    public int AINum;

    private CameraChange _cameraChangeScript;
    private GameObject _followerTarget;
    private GameObject _follower;

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

    public void LoadGameObjects()
    {
        if (MirrorManager == null)
            MirrorManager = GetVNM();
        if (MainRig == null)
            MainRig = (GameObject)Resources.Load("Prefabs/Network/MainRigAll");
        if (_randomCharacterPlacerScript == null)
            _randomCharacterPlacerScript = GameObject.Find("People").GetComponent<RandomCharacterPlacer>();
        if (_cameraChangeScript == null)
            _cameraChangeScript = GameObject.Find("CameraGroups").GetComponent<CameraChange>();
        if (_follower == null)
            _follower = GameObject.Find("Follower");
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
        if (MirrorManager != null && MirrorManager.IsClient())
        {
            CameraFollow(true, _followerTarget);
        }
    }

    public void  UpdateUI(int playerNum, int aiNum)
    {
        var _ccuTex = GameObject.Find("Counter").GetComponent<Text>();
        _ccuTex.text = "Player: " + playerNum + " AI:" + aiNum;
    }

    IEnumerator NetworkManagerSpawnAnimals()
    {
        yield return new WaitForSeconds((Random.Range(0, 200) / 100));
        ServerHandler.SpawnAIs(null, new VirtualRequest());
        
    }
    private void CameraFollow(bool flag, GameObject target)
    {
        if (target == null)
        {
            target = MirrorManager.GetConnPlayer();
        };
        if (target != null)
        {
            _cameraChangeScript.CameraFollow(_follower.transform, target.transform, new Vector3(0, 1.8f, 0));
            _cameraChangeScript.CameraSwitch(flag);
        }
    }


}
