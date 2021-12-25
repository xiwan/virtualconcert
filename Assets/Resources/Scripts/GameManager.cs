using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // private static GameManager _instance;

    // public static GameManager getInstance()
    // {
    //     if (_instance == null)
    //         _instance = new GameManager();

    //     return _instance;
    // }

    MonoBehaviour _cameraChangeScript;
    public int selectedPlayer = 0;

    public int lastSelectedPlayer = 0;

    public bool resetCamera = false;

    public bool pickAnyPlayer = false;

    public Dictionary<int, int> selectedPlayerDict = new Dictionary<int, int>();

    public void SelectPlayer(int playerId)
    {
        //selectedPlayerQueue.Enqueue(playerId);
        //selectedPlayerDict[playerId] = 1;
        lastSelectedPlayer = selectedPlayer;
        selectedPlayer = playerId;
        if (lastSelectedPlayer != selectedPlayer)
        {
            PlayerPool.getInstance().resetDataExcept(selectedPlayer);
        }
    }

    public Player PickAnyPlayer()
    {
        var player = PlayerPool.getInstance().GetAnyPlayer();
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
            PlayerPool.getInstance().resetDataExcept(playerId);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _cameraChangeScript = GameObject.Find("CameraGroups").GetComponent<CameraChange>();
        
        //StartCoroutine(InitTask());
    }

    // Update is called once per frame
    void Update()
    {
        NextTask();
    }

    private IEnumerator InitTask()
    {
        yield return new WaitForSeconds((Random.Range(0, 200) / 100));
        NextTask();
    }

    private void NextTask()
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
            var player = PlayerPool.getInstance().GetPlayer(selectedPlayer);
            if (player != null)
            {
                var follower = player.Follower;
                var target = player.MoveController;
                ((CameraChange)_cameraChangeScript).CameraFollow(follower.transform, target.transform, new Vector3(0, 1.8f, 0));
                ((CameraChange)_cameraChangeScript).CameraSwitch(true);
            }
             
        }
        //NextTask();
    }

    

}
