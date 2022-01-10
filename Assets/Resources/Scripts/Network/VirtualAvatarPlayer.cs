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

    public bool takeOver = false;

    public float groundCheckRadius = 0.2f;
    public float gravity = -9.8f;
    public float speed = 5f;
    public float rotateSpeed = 1f;
    public float jumpHeight = 3f;
    public LayerMask layerMask;

    private Player _player;

    private string _swapAnimatorPath = "AnimationControllers/UserController";
    private bool _isJumping = false;
    private bool _isWalking = false;
    private bool _isRunning = false;
    private bool _isDancing = false;
    private bool _isGrounded = false;
    private Transform _groundCheck;

    private AnimatorOverrideController _overrideController;
    private AnimatorOverrideController _currentController;
    private CharacterController _characterController;
    //private CameraChange _cameraChangeScript;
    private WanderScript _wanderScript;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    private Vector3 _velocity = Vector3.zero;
    public MoveData _moveData;

    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();
        _overrideController = Resources.Load<AnimatorOverrideController>(_swapAnimatorPath);

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

        _wanderScript.enabled = false;
        _navMeshAgent.enabled = false;
        var spawnedInstance = AvatarManager.Instance.SpawnFromAvatar(this.gameObject, avatar);
        //Debug.Log(this.gameObject.name);
        
        if (avatar.type == CHARACTER.Player)
        {
            var parent = GameObject.Find("People/Players");
            this.transform.SetParent(parent.transform, false);
            this.takeOver = true;
            TakeOverEventOn();
        }
        if (avatar.type == CHARACTER.AI)
        {
            var parent = GameObject.Find("People/AIs");
            this.transform.SetParent(parent.transform, false);
            this.takeOver = false;
        }

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _moveData = new MoveData();
        if (_player == null)
        {
            this.takeOver = false;
            _player = gameObject.AddComponent<Player>();
            _player.instanceId = GetInstanceID();
            //_player.moveController = _moveController;
            _player.playerController = this;
            _player.takeOver = this.takeOver;

            // register AI player
            if (avatar.type == CHARACTER.AI)
            {
                _player.networkId = -1;
            }
            // register real player
            else if (avatar.type == CHARACTER.Player)
            {
                _player.networkId = Convert.ToInt32(netIdentity.netId);
                PlayerPoolManager.Instance.UpsertData(_player.networkId, _player);
            }
        }

    }

    public void Update()
    {
        if (isClient)
        {
            ClientUpdate();
        }

        else if (isServer)
        {
            ServerUpdate();
        }
    
    }

    private void ClientUpdate()
    {
        ClientAuthMove(takeOver);
    }

    private void ServerUpdate()
    {
        if (takeOver)
        {
            TakeOverEventOn();

            MoveLikeWoW();
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
                //Debug.Log(_overrideController);
                _animator.runtimeAnimatorController = _overrideController;
            }
        }

        //GameManager.GetGM().SelectPlayer(_player.networkId);
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
                //Debug.Log(_currentController);
                _animator.runtimeAnimatorController = _currentController;
            }
        }
        //GameManager.GetGM().DeselectPlayer(_player.networkId);
        //StartCoroutine(OutlineCharacter(0));
    }

    private void ClientAuthMove(bool takeOver)
    {
        MoveLikeWoW();
        
    }

    private void ServerAuthMove()
    {
        //StartCoroutine(MoveCharacter(netIdentity.netId, false));
    }

    private void MoveLikeWoW()
    {
        if (isClient && connectionToServer != null)
        {
            MoveCharacter(netIdentity.netId, takeOver);
            CameraFollow(takeOver);
            StartCoroutine(OutlineCharacter(0.02f));
        }
        else if (isServer)
        {
            _characterController.Move(_moveData.hmove);
            _characterController.Move(_moveData.vmove);
            transform.Rotate(Vector3.up, _moveData.angle);
        }
    }


    [Command]
    public void SpawnAIsOnServer()
    {
        GameManager.GetGM().SpawnAnimals();
    }

    [Command]
    public void PickAnyAIOnServer()
    {
        //GameManager.GetGM().PickAnyPlayer();

    }

    private IEnumerator OutlineCharacter(float value)
    {
        yield return new WaitForSeconds((UnityEngine.Random.Range(0, 200) / 100));
        Material[] materials = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        materials[1].SetFloat("_OutlineFactor", value);
    }

    private IEnumerator MoveCharacterAnimation(MoveData moveData)
    {
        yield return new WaitForSeconds((UnityEngine.Random.Range(0, 200) / 100));

        var _speed = 0f;
        if (moveData.horizontal != 0 || moveData.vertical != 0)
        {
            _animator.SetBool("isBlending", true);
            _speed = 0.5f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _speed = 0.9f;
            }
        }
        else
        {
            _animator.SetBool("isBlending", false);
        }

        _animator.SetFloat("Speed", _speed);
        _animator.SetBool("isJumping", _isJumping);
        _animator.SetBool("isDancing", _isDancing);
    }

    private void CameraFollow(bool flag)
    {
        var _cameraChangeScript = GameObject.Find("CameraGroups").GetComponent<CameraChange>();
        var follower = GameObject.Find("Follower");
        var target = this;

        _cameraChangeScript.CameraFollow(follower.transform, target.transform, new Vector3(0, 1.8f, 0));
        _cameraChangeScript.CameraSwitch(flag);
    }

    private void MoveCharacter(uint netId, bool value = false)
    {
        if (connectionToServer == null) return;

        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        _velocity.y += gravity * Time.deltaTime;


        if (Input.GetButtonDown("Jump"))
        {
            if (isGround(_groundCheck) && !_isJumping)
            {
                _velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
                _isJumping = true;
            }
            else
            {
                _isJumping = false;
            }
        }
        else if (Input.GetKey(KeyCode.E))
        {
            if (isGround(_groundCheck) && !_isDancing)
            {
                _isDancing = true;
            }
            else
            {
                _isDancing = false;
            }
        }


        var _moveData = new MoveData
        {
            horizontal = h,
            vertical = v,
            hmove = transform.forward * speed * v * Time.deltaTime,
            vmove = _velocity * Time.deltaTime,
            angle = h * rotateSpeed,
        };

        var msg = new VirtualRequest
        {
            messageId = ServerMsgType.ClientTakeOver,
            networkId = Convert.ToInt32(netId),
            takeOver = value,
            moveData = _moveData
        };
        connectionToServer.Send(msg);

        StartCoroutine(MoveCharacterAnimation(_moveData));
   
    }

    private bool isGround(Transform obj)
    {
        if (obj == null)
            obj = transform.Find("GroundCheck");
        _isGrounded = Physics.CheckSphere(obj.position, groundCheckRadius, layerMask);
        return _isGrounded;
    }


}

