using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using PolyPerfect;

public class MoveController : MonoBehaviour
{
    private CharacterController _characterController;
    private MonoBehaviour _wanderScript;
    private GameManager _gm;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    //private GameObject _follower;

    private AnimatorOverrideController _overrideController;
    private AnimatorOverrideController _currentController;
    private Transform _groundCheck;
    private bool _isGrounded = false;

    private string _swapAnimatorPath = "AnimationControllers/UserController";
    private bool _isJumping = false;
    private bool _isWalking = false;
    private bool _isRunning = false;
    private bool _isDancing = false;
    private Vector3 _velocity = Vector3.zero;

    private Player _player;

    public float groundCheckRadius = 0.2f;
    public float gravity = -9.8f;
    public float speed = 5f;
    public float rotateSpeed = 1f;
    public float jumpHeight = 3f;
    public bool takeOver = false;
    public LayerMask layerMask;
    private EventManager _eventManager;
    
    // Take Over Authority On Event
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

        if (_animator != null )
        {
            if (_overrideController != null && _overrideController != _animator.runtimeAnimatorController)
            {
                Debug.Log(_overrideController);
                _animator.runtimeAnimatorController = _overrideController;
            }   
        }

        _gm.SelectPlayer(_player.InstanceId);
        StartCoroutine(OutlineCharacter(0.02f));
    }

    // Take Over Authority Off Event
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
            if (_currentController != null && _currentController != _animator.runtimeAnimatorController)
            {
                Debug.Log(_currentController);
                _animator.runtimeAnimatorController = _currentController;
            }
        }
        _gm.DeselectPlayer(_player.InstanceId);
        StartCoroutine(OutlineCharacter(0));
    }

    private void Awake()
    {
        _gm = GameManager.GetGM();

        if (_player == null)
        {
            _player = new Player();
            _player.InstanceId = GetInstanceID();
            _player.MoveController = this;
            _player.TakeOver = false;
            _player.Follower = GameObject.Find("Follower");
 
            // register player
            PlayerPool.GetInstance().UpsertData(_player.InstanceId, _player);
        }

        if (_eventManager == null)
        {
            _eventManager = new EventManager();
            // register event
            _eventManager.AddHandler(EVENT.TakeOverEventOn, () =>{
                TakeOverEventOn();
                MoveLikeWoW();
                MoveCustom();
            });
            _eventManager.AddHandler(EVENT.TakeOverEventOff, () => {
                TakeOverEventOff();             
            }); 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = transform.GetComponent<Animator>();

        _overrideController = Resources.Load<AnimatorOverrideController>(_swapAnimatorPath);
        _currentController = new AnimatorOverrideController();
        _currentController = (AnimatorOverrideController)_animator.runtimeAnimatorController;

        _characterController = transform.GetComponent<CharacterController>();
        _wanderScript = transform.GetComponent<WanderScript>();
        _navMeshAgent = transform.GetComponent<NavMeshAgent>();
        _groundCheck = transform.Find("GroundCheck");
    }

    // Update is called once per frame
    void Update()
    {
        if (takeOver)
        {
            _eventManager?.Trigger(EVENT.TakeOverEventOn);
        }
        else
        {
            _eventManager?.Trigger(EVENT.TakeOverEventOff);
        }
    }

    private void FixedUpdate()
    {
  
    }

    private void MoveLikeWoW()
    {

        var _horizontal = Input.GetAxis("Horizontal");
        var _vertical = Input.GetAxis("Vertical");

        var _move = transform.forward * speed * _vertical * Time.deltaTime;
        _characterController.Move(_move);

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        transform.Rotate(Vector3.up, _horizontal * rotateSpeed);

        var _speed = 0f;
        if (_horizontal != 0 || _vertical != 0)
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

    }

    private void MoveCustom()
    {
        // jump
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
            _animator.SetBool("isJumping", _isJumping);             
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
            _animator.SetBool("isDancing", _isDancing);
        }    
    }

    private bool isGround(Transform obj)
    {
        _isGrounded = Physics.CheckSphere(obj.position, groundCheckRadius, layerMask);
        return _isGrounded;
    }

    private IEnumerator OutlineCharacter(float value)
    {
        yield return new WaitForSeconds((UnityEngine.Random.Range(0, 200) / 100));
        Material[] materials = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        materials[1].SetFloat("_OutlineFactor", value);
    }


}

