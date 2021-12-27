using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolController : MonoBehaviour
{
    private Animator _animator; // 主角身上的状态机  
    private AnimatorStateInfo _currentStateInfo; // 当前动画信息  
    private AnimatorStateInfo _PreStateInfo;     // 上一次的动画信息

    private CharacterController _characterController;
    private Vector3 _velocity = Vector3.zero;
    public float gravity = -9.8f;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = transform.GetComponent<CharacterController>();
        _animator = GetComponent<Animator>(); // 获取当前状态机组件  
        _currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0); //获取第0层的动画保存到当前动画信息  
        _PreStateInfo = _currentStateInfo;  // 将当前动画信息赋值给上一个动画信息 

        StartCoroutine(KeepDancing()); // 开启协同程序
    }

    // Update is called once per frame
    void Update()
    {
        //_velocity.y += gravity * Time.deltaTime;
        //_characterController.Move(_velocity * Time.deltaTime);

        
    }

    IEnumerator KeepDancing() // 一开始就执行人物动画持续播放方法
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // 等待1秒后再次执行该方法
            _animator.SetBool("isDancing", true);
            //string aniName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            //Debug.Log(aniName);
            //if (_currentStateInfo.IsName("ThrillDance01"))
            //{
             //   _animator.SetBool("isIdling", true);
             //   _animator.SetBool("isIdling", false);
            //}
        }
    }

}
