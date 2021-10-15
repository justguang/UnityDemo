using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹
/// </summary>
[AddComponentMenu("MyScript/Rocket")]
public class Rocket : MonoBehaviour
{
    public float m_speed = 10;//子弹移速
    public float m_power = 1.0f;//子弹威力
    protected Transform m_transform;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //子弹向前移动
        m_transform.Translate(0, 0, m_speed * Time.deltaTime);
    }

    /// <summary>
    /// unity func：当该物体在任何相机上都不可见时调用 OnBecameInvisible
    /// </summary>
    private void OnBecameInvisible()
    {
        //当离开屏幕后，如果任处于激活状态，就销毁
        //if (this.enabled)
        //Destroy(this.gameObject);

        Despawn();
    }

    /// <summary>
    /// unity func：在碰撞体互相接触时触发此方法
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //Destroy(this.gameObject);
            Despawn();
        }
    }

    /// <summary>
    /// 自我回收
    /// </summary>
    public void Despawn()
    {
        if (!gameObject.activeSelf) return;//如果未激活，return

        //利用对象池进行回收
        var p = PoolManager.Pools["mypool"];
        if (p.IsSpawned(m_transform))//判断该对象是否在对象池中
        {
            p.Despawn(m_transform);//回收于对象池内
        }
        else
        {
            Destroy(gameObject);//直接销毁不在对象池内的游戏对象
            Debug.LogWarning($"Destroy销毁{this.name}");
        }
    }

}
