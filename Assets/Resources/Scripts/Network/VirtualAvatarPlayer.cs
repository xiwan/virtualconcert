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
            StartCoroutine(OutlineCharacter(0.02f));
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
        
    
    }

    public void FixedUpdate()
    {
        OnSimulateBefore();
        Simulate();
        OnSimulateAfter();
    }

    void Simulate()
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

    void OnSimulateBefore()
    {
        if (isClient)
        {
            
        }
        else if (isServer)
        {

        }
    }

    void OnSimulateAfter()
    {
        if (takeOver)
        {
            // server & client 
            MoveAnimation(_moveData);
            
        }
    }

    private void ClientUpdate()
    {
        if (takeOver)
        {
            MoveLikeWoW();
        }
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

    private void MoveLikeWoW()
    {
        if (isClient && connectionToServer != null)
        {
            MoveCharacter(netIdentity.netId);
        }
        else if (isServer)
        {
            if (_moveData.walk)
            {
                var h = _moveData.horizontal;
                var v = _moveData.vertical;
                _velocity.y += gravity * Time.fixedDeltaTime;

                _characterController.Move(transform.forward * speed * v * Time.fixedDeltaTime);
                _characterController.Move(_velocity * Time.fixedDeltaTime);
                transform.Rotate(Vector3.up, h * rotateSpeed * 2);

            }
        }
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

    public void MoveAnimation(MoveData moveData)
    {
        if (_moveData == null) return;

        if (_moveData.walk)
        {
            _isWalking = true;
            _isJumping = false;
            _isDancing = false;

        }
        else if (_moveData.jump)
        {
            _isJumping = (IsGround(_groundCheck) && !_isJumping);
            if (_isJumping)
            {
                //_velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);

                _isWalking = false;
                _isJumping = true;
                _isDancing = false;
            }
        }
        else if (_moveData.dance)
        {
            _isDancing = (IsGround(_groundCheck) && !_isDancing);
            if (_isDancing)
            {
                _isWalking = false;
                _isJumping = false;
                _isDancing = true;
            }
        }

        _animator.SetFloat("Speed", moveData.speed);
        _animator.SetBool("isBlending", moveData.speed > 0);
        _animator.SetBool("isJumping", _isJumping);
        _animator.SetBool("isDancing", _isDancing);
    }

    private void MoveCharacter(uint netId)
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        _moveData = new MoveData
        {
            horizontal = h,
            vertical = v,
            speed = 0,
            walk = (h != 0 || v != 0),
            jump = Input.GetKey(KeyCode.Space),
            dance = Input.GetKey(KeyCode.E),
            sprint = Input.GetKey(KeyCode.LeftShift),
        };

        if (_moveData.walk)
        {
            _moveData.speed = (_moveData.sprint) ? 0.9f : 0.5f;
        }

        var msg = new VirtualRequest
        {
            messageId = ServerMsgType.ClientTakeOver,
            networkId = Convert.ToInt32(netId),
            takeOver = true,
            moveData = _moveData
        };
        connectionToServer.Send(msg);
   
    }

    private bool IsGround(Transform obj)
    {
        if (obj == null)
            obj = transform.Find("GroundCheck");
        _isGrounded = Physics.CheckSphere(obj.position, groundCheckRadius, layerMask);
        return _isGrounded;
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

    }

}

