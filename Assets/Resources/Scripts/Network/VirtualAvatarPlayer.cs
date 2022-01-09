using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using PolyPerfect;


public enum CHARACTER
{
    AI,
    Player,
    IDOL
};

public class VirtualAvatarPlayer : NetworkBehaviour
{
    [SyncVar]
    public Avatar avatar;
    [SyncVar]
    public bool takeOver = false;

    private string _swapAnimatorPath = "AnimationControllers/UserController";
    private AnimatorOverrideController _overrideController;
    private AnimatorOverrideController _currentController;
    private MoveController _moveController;
    private CharacterController _characterController;
    private MonoBehaviour _wanderScript;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;


    private Player _player;


    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();
        Debug.Log(_animator.runtimeAnimatorController.name);
        _overrideController = Resources.Load<AnimatorOverrideController>(_swapAnimatorPath);

        _moveController = transform.GetComponent<MoveController>();
        _characterController = transform.GetComponent<CharacterController>();
        _wanderScript = transform.GetComponent<WanderScript>();
        _navMeshAgent = transform.GetComponent<NavMeshAgent>();

    }

    public void SetCurrentState()
    {
        _currentController = new AnimatorOverrideController();
        _currentController = (AnimatorOverrideController) _animator.runtimeAnimatorController;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (avatar.type == CHARACTER.Player)
        {
            var parent = GameObject.Find("People/Players");
            this.transform.SetParent(parent.transform, false);
        }
        if (avatar.type == CHARACTER.AI)
        {
            var parent = GameObject.Find("People/AIs");
            this.transform.SetParent(parent.transform, false);
        }
        _wanderScript.enabled = false;
        var spawnedInstance = AvatarManager.Instance.SpawnFromAvatar(this.gameObject, avatar);
        Debug.Log(this.gameObject.name);

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (_player == null)
        {
            _player = gameObject.AddComponent<Player>();
            _player.instanceId = GetInstanceID();
            _player.moveController = _moveController;
            _player.takeOver = false;
            _player.follower = GameObject.Find("Follower");

            // register AI player
            if (avatar.type == CHARACTER.AI)
            {
                PlayerPoolManager.Instance.UpsertData(_player.instanceId, _player);
            }
            if (avatar.type == CHARACTER.Player)
            {

            }
        }

    }

    public void Update()
    {
        if (isClient)
        {
            ClientUpdate();
        }

        if (isServer)
        {
            ServerUpdate();
        }
    
    }

    private void ClientUpdate()
    {
        if (takeOver)
        {
            StartCoroutine(OutlineCharacter(0.02f));
        }
        else
        {
            StartCoroutine(OutlineCharacter(0.0f));
        }
    }

    private void ServerUpdate()
    {
        if (takeOver)
        {
            TakeOverEventOn();
        }
        else
        {
            TakeOverEventOff();
        }
    }


    public void TakeOverEventOn()
    {
        if (_wanderScript != null && _wanderScript.isActiveAndEnabled)
        {
            _wanderScript.enabled = false;
            _wanderScript.StopAllCoroutines();
        }
        if (_navMeshAgent != null && _navMeshAgent.isActiveAndEnabled)
        {
            _navMeshAgent.enabled = false;
        }

        if (_animator != null)
        {
            if (_overrideController != null && _overrideController.runtimeAnimatorController != _animator.runtimeAnimatorController)
            {
                Debug.Log(_overrideController);
                _animator.runtimeAnimatorController = _overrideController;
            }
        }

        GameManager.GetGM().SelectPlayer(_player.instanceId);
        //StartCoroutine(OutlineCharacter(0.02f));
    }

    public void TakeOverEventOff()
    {
        if (_wanderScript != null && !_wanderScript.isActiveAndEnabled)
        {
            _wanderScript.enabled = true;
            _wanderScript.Invoke("StartWander", (UnityEngine.Random.Range(0, 200) / 100));
        }
        if (_navMeshAgent != null && !_navMeshAgent.isActiveAndEnabled)
        {
            _navMeshAgent.enabled = true;
        }

        if (_animator != null)
        {
            if (_currentController != null && _currentController.runtimeAnimatorController != _animator.runtimeAnimatorController)
            {
                Debug.Log(_currentController);
                _animator.runtimeAnimatorController = _currentController;
            }
        }
        GameManager.GetGM().DeselectPlayer(_player.instanceId);
        //StartCoroutine(OutlineCharacter(0));
    }



    [Command]
    public void SpawnAIsOnServer()
    {
        GameManager.GetGM().SpawnAnimals();
    }


    private IEnumerator OutlineCharacter(float value)
    {
        yield return new WaitForSeconds((UnityEngine.Random.Range(0, 200) / 100));
        Material[] materials = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        materials[1].SetFloat("_OutlineFactor", value);
    }


}

