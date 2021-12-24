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
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Camera _mainCamera;
    private CinemachineVirtualCamera _followCamera;
    private CinemachineFreeLook _freeCamera;
    private GameObject _follower;

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

    public float groundCheckRadius = 0.2f;
    public float gravity = -9.8f;
    public float speed = 5f;
    public float rotateSpeed = 1f;
    public float jumpHeight = 3f;
    public bool takeOver = false;
    public LayerMask layerMask;
    

    private void Awake()
    {
        _animator = transform.GetComponent<Animator>();

        _overrideController = Resources.Load<AnimatorOverrideController>(_swapAnimatorPath);
        Debug.Log(_overrideController);

        _currentController = new AnimatorOverrideController();
        _currentController = (AnimatorOverrideController)_animator.runtimeAnimatorController;
        Debug.Log(_currentController);

    }

    // Start is called before the first frame update
    void Start()
    {
        _characterController = transform.GetComponent<CharacterController>();
        _wanderScript = transform.GetComponent<WanderScript>();
        _navMeshAgent = transform.GetComponent<NavMeshAgent>();
        _groundCheck = transform.Find("GroundCheck");

        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _followCamera = GameObject.Find("CM vcam").GetComponent<CinemachineVirtualCamera>();
        _freeCamera = GameObject.Find("CM FreeLook").GetComponent<CinemachineFreeLook>();
        _follower = GameObject.Find("Follower");
        
    }

    // Update is called once per frame
    void Update()
    {
        if (takeOver)
        {
            if (_wanderScript != null && _wanderScript.isActiveAndEnabled)
            {
                _wanderScript.enabled = false;
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
            MoveLikeWoW();
            MoveCustom();
        }
        else
        {
            if (_wanderScript != null && !_wanderScript.isActiveAndEnabled)
            {
                _wanderScript.enabled = true; 
                _wanderScript.SendMessage("StartWander");              
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
        }
    }

    private void FixedUpdate()
    {
        
        if (takeOver)
        {
            if (_follower != null)
            {
                FollowObject(_follower.transform, transform, new Vector3(0, 1.8f, 0));
            }
        }
        else
        {
            if (_follower != null)
            {
                //FollowObject(_follower.transform, _freeCamera.transform, Vector3.zero);
            }
        }
        
    }

    private void FollowObject(Transform follower, Transform target, Vector3 offset)
    {
        float distance = Vector3.Distance(follower.position, target.position);
        follower.position = Vector3.MoveTowards(follower.position, target.position + offset, Time.deltaTime * 100);
        
        var direction = target.position - follower.position;
        var rotation = Quaternion.LookRotation(direction);
        follower.rotation = Quaternion.LookRotation(target.forward);
    }

    private void MoveLikeWoW()
    {
        // if (isGround(_groundCheck) && _velocity.y < 0)
        // {
        //     _velocity.y = 0;
        // }

        var _horizontal = Input.GetAxis("Horizontal");
        var _vertical = Input.GetAxis("Vertical");

        var _move = transform.forward * speed * _vertical * Time.deltaTime;
        _characterController.Move(_move);

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

        transform.Rotate(Vector3.up, _horizontal * rotateSpeed);

        if (_horizontal != 0 || _vertical != 0)
        {
            _animator.SetBool("isBlending", true);
            _animator.SetFloat("Speed", 0.9f);
        }
        else
        {
            _animator.SetBool("isBlending", false);
            _animator.SetFloat("Speed", 0f);
        }

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
                //_animator.SetBool("isJumping", true);
            }
            else
            {
                _isJumping = false;
                //_animator.SetBool("isJumping", false);
                //_animator.SetBool("isDancing", false); 
                //_animator.SetBool("isBlending", false); 
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
                //_animator.SetBool("isDancing", false);
                //_animator.SetBool("isJumping", false);
                //_animator.SetBool("isBlending", false);   
            }
            _animator.SetBool("isDancing", _isDancing);
        }
        
    }

    private bool isGround(Transform obj)
    {
        // Vector3 fwd = transform.TransformDirection(-Vector3.up);
        // bool grounded =  Physics.Raycast(transform.position,fwd, 10 );
        // return grounded;
        _isGrounded = Physics.CheckSphere(obj.position, groundCheckRadius, layerMask);
        return _isGrounded;
    }

    private IEnumerator StartAnimation()
    {
        yield return new WaitForSeconds((Random.Range(0, 200) / 100));
        
    }
    
}

