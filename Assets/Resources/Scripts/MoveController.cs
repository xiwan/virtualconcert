using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace PolyPerfect
{
    public class MoveController : MonoBehaviour
    {
        private CharacterController _characterController;
        private MonoBehaviour _wanderScript;
        private Animator _animator;
        private Camera _mainCamera;
        private CinemachineVirtualCamera _followCamera;
        private GameObject _follower;

        private AnimatorOverrideController _overrideController;
        private AnimatorOverrideController _currentController;
        private string _swapAnimatorPath = "AnimationControllers/UserController";
        private bool _isJumping = false;
        private bool _isWalking = false;
        private bool _isRunning = false;
        private bool _isDancing = false;

        public float speed = 5f;
        public float rotateSpeed = 1f;
        public bool takeOver = false;
        

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
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            _followCamera = GameObject.Find("CM vcam").GetComponent<CinemachineVirtualCamera>();
            _follower = GameObject.Find("Follower");
        }

        // Update is called once per frame
        void Update()
        {
            if (takeOver)
            {
                if (_wanderScript != null && _wanderScript.isActiveAndEnabled)
                    _wanderScript.enabled = false;
                if (_animator != null )
                {
                    if (_overrideController != null && _overrideController != _animator.runtimeAnimatorController)
                    {
                        Debug.Log(_overrideController);
                        _animator.runtimeAnimatorController = _overrideController;
                        _animator.SetBool("isIdling", true);
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
                }
                if (_animator != null)
                {
                    if (_currentController != null && _currentController != _animator.runtimeAnimatorController)
                    {
                        Debug.Log(_currentController);
                        _animator.runtimeAnimatorController = _currentController;
                        _animator.SetBool("isIdling", true);
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
                     //FollowObject(_follower.transform, _mainCamera.transform);
                }
            }
            
        }

        private void FollowObject(Transform follower, Transform target, Vector3 offset)
        {
            float distance = Vector3.Distance(follower.position, target.position);
            var direction = target.position - follower.position;
            follower.position = Vector3.MoveTowards(follower.position, target.position + offset, Time.deltaTime * 10);
            
            var rotation = Quaternion.LookRotation(direction);
            follower.rotation = Quaternion.LookRotation(target.forward);
        }

        private void MoveLikeWoW()
        {
            
            var _horizontal = Input.GetAxis("Horizontal");
            var _vertical = Input.GetAxis("Vertical");

            var _move = transform.forward * speed * _vertical * Time.deltaTime;
            _characterController.Move(_move);

            transform.Rotate(Vector3.up, _horizontal * rotateSpeed);

        }

        private void MoveCustom()
        {
            // jump
            if (Input.GetKey(KeyCode.Space))
            {
                if (isGround(transform) && !_isJumping)
                {
                    _isJumping = true; 
                    _animator.SetBool("isJumping", true);
                }
                else
                {
                    _isJumping = false;
                    _animator.SetBool("isJumping", false);
                    _animator.SetBool("isDancing", false);  
                }               
            }
            else if (Input.GetKey(KeyCode.E))
            {
                if (isGround(transform) && !_isDancing)
                {
                    _isDancing = true; 
                    _animator.SetBool("isDancing", true);
                }
                else
                {
                    _isDancing = false;
                    _animator.SetBool("isDancing", false);
                    _animator.SetBool("isJumping", false);  
                } 
            }
           
        }

        private bool isGround(Transform obj)
        {
            Vector3 fwd = transform.TransformDirection(-Vector3.up);
            bool grounded =  Physics.Raycast(transform.position,fwd, 10 );
            return grounded;
        }
    }

}
