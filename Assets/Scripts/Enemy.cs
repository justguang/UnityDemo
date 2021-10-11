using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 敌人
/// </summary>
[AddComponentMenu("MyScript/Enemy")]
public class Enemy : MonoBehaviour
{

    public float m_speed = 1;//敌人飞行速度
    public float m_life = 2;//敌人生命
    protected int m_point;//敌人分数
    protected float m_rotSpeed = 30;//敌人旋转速度

    internal Renderer m_renderer;//模型渲染组件
    internal bool m_isActiv = false;//是否激活

    protected Transform m_transform;

    public AudioClip m_shootClip;//射击声音
    protected AudioSource m_audio;//声音源
    public Transform m_explosionFX;//爆炸特效

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        m_renderer = this.GetComponent<Renderer>();
        m_audio = this.GetComponent<AudioSource>();
        m_point = (int)m_life;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();
        if (m_isActiv && !this.m_renderer.isVisible)
        {
            //当任处于激活状态，但已经不被任何相机渲染时，自我销毁
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 为了将来扩展功能，此方法为虚方法
    /// </summary>
    protected virtual void UpdateMove()
    {
        //Time.time是游戏进行时间，Math.Sin函数使数值在 -1~1之间循环变化，最终得到可以左右移动的值
        float rx = Mathf.Sin(Time.time) * Time.deltaTime;

        m_transform.Translate(new Vector3(rx, 0, -m_speed * Time.deltaTime));
    }

    /// <summary>
    /// unity func：在碰撞体互相接触时触发此方法
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerRocket")//如果撞到tag为PlayerRocket【主角子弹】的物体
        {
            Rocket rocket = other.GetComponent<Rocket>();//获得子弹上的脚本组件
            if (rocket != null)
            {
                m_life -= rocket.m_power;//减少生命
                if (m_life <= 0)
                {

                    //播放爆炸特效
                    Transform fx = Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);

                    GameManager.Instance.DelayDestoryFx(2, fx.gameObject);//2秒后销毁爆炸特效
                    GameManager.Instance.addScore(m_point);//分数增加

                    //如果生命<=0，自我销毁
                    Destroy(this.gameObject);
                }
            }
        }
        else if (other.tag == "Player")//如果撞到主角
        {
            //播放爆炸特效
            Transform fx = Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
            m_life = 0;
            GameManager.Instance.DelayDestoryFx(2, fx.gameObject);//2秒后销毁爆炸特效
            Destroy(this.gameObject);//自我销毁
        }
    }

    /// <summary>
    /// unity func：当出现在屏幕中时触发
    /// </summary>
    private void OnBecameVisible()
    {
        m_isActiv = true;
    }

    /// <summary>
    /// unity func：当离开在屏幕上时触发
    /// </summary>
    private void OnBecameInvisible()
    {
    }
}
