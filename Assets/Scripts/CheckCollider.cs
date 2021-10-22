using System;
using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    /// <summary>
    /// 检测到玩家靠近后的通知回调
    /// </summary>
    public event Action<string, Transform> triggerEnterCallBack;


    /// <summary>
    /// 检测到玩家远离的通知回调
    /// </summary>
    public event Action<string, Transform> triggerLeaveCallBack;


    void Start()
    {
        ZombieController ctrl = transform.GetComponentInParent<ZombieController>();
        if (ctrl != null)
        {
            triggerEnterCallBack += ctrl.CheckEnter;
            triggerLeaveCallBack += ctrl.CheckLeave;
        }
        else
        {
            Debug.LogError($"僵尸的{transform.name}获取ZombieController脚本失败");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            //检测到玩家
            if (triggerEnterCallBack != null)
                triggerEnterCallBack(transform.name, other.transform);
            else
                Debug.LogError($"僵尸的{gameObject.name}检测触发，但是没有订阅事件");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            //检测到玩家
            if (triggerLeaveCallBack != null)
                triggerLeaveCallBack(transform.name, other.transform);
            else
                Debug.LogError($"僵尸的{gameObject.name}检测触发，但是没有订阅事件");
        }
    }

}
