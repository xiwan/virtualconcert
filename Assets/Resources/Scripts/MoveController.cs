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
        private string _swapAnimatorPath = "AnimationControllers/BrooklynuprockController";

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
                        _animator.SetBool("isDancing", true);
                    }   
                }
                
                MoveLikeWoW();
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
                        _animator.SetBool("isDancing", true);
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
                    //_follower.transform.SetParent(this.transform);
                    _follower.transform.position = Vector3.Lerp(_follower.transform.position, transform.position + new Vector3(0, 1.8f, 0), 0.1f);
                    _follower.transform.LookAt(transform.forward, transform.up);
                    //_follower.transform.Translate(transform.up * 1 * Time.deltaTime, Space.World);
                }
            }
            else
            {
                if (_follower != null)
                {
                    //_follower.transform.position = Vector3.Lerp(_follower.transform.position, _mainCamera.transform.position, 0.1f);
                    //_follower.transform.LookAt(_mainCamera.transform.forward, _mainCamera.transform.up);
                }
            }
            
        }

        private void MoveLikeWoW()
        {
            var _horizontal = Input.GetAxis("Horizontal");
            var _vertical = Input.GetAxis("Vertical");

            var _move = transform.forward * speed * _vertical * Time.deltaTime;
            _characterController.Move(_move);

            transform.Rotate(Vector3.up, _horizontal * rotateSpeed);

        }
    }

}
