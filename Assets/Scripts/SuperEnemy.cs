using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 强大的敌人
/// </summary>
[AddComponentMenu("MyScript/SuperEnemy")]
public class SuperEnemy : Enemy
{
    protected float m_fireTime = 0;//射击计时器
    protected Transform m_player;//主角


    protected override void UpdateMove()
    {
        m_fireTime -= Time.deltaTime;
        if (m_fireTime <= 0)
        {
            m_fireTime = 2;
            Shoot();
        }

        //直线向主角移动【z轴负方向】
        m_transform.Translate(new Vector3(0, 0, -m_speed * Time.deltaTime));
    }

    /// <summary>
    /// 射击
    /// </summary>
    void Shoot()
    {
        if (m_player != null)
        {
            //使用向量减法获取朝向主角位置的方向（目标位置-自身位置）
            Vector3 relativePos = m_player.position - m_transform.position;

            //Instantiate(m_rocket, m_transform.position, Quaternion.LookRotation(relativePos));

            //利用对象池生成子弹
            var p = PoolManager.Pools["mypool"];
            p.Spawn("EnemyRocket", m_transform.position, Quaternion.LookRotation(relativePos), null);

            //播放射击声音
            m_audio.PlayOneShot(m_shootClip);
        }
        else
        {
            //查找主角
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
            {
                m_player = obj.transform;
            }
        }
    }

    /// <summary>
    /// unity func：在碰撞体互相碰撞时触发此方法
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "PlayerRocket")//如果撞到tag为PlayerRocket【主角子弹】的物体
        {
            Rocket rocket = other.gameObject.GetComponent<Rocket>();//获得子弹上的脚本组件
            if (rocket != null)
            {
                m_life -= rocket.m_power;//减少生命
                if (m_life <= 0)
                {

                    //播放爆炸特效
                    //Transform fx = Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
                    Transform fx = PoolManager.Pools["mypool"].Spawn("Explosion", m_transform.position, Quaternion.identity);

                    //GameManager.Instance.DelayDestoryFx(2, fx.gameObject);//2秒后销毁爆炸特效
                    PoolManager.Pools["mypool"].Despawn(fx, 2);

                    GameManager.Instance.addScore(m_point);//分数增加

                    //如果生命<=0，自我销毁
                    //Destroy(this.gameObject);
                    Despawn();
                }
            }
        }
        else if (other.gameObject.tag == "Player")//如果撞到主角
        {
            //播放爆炸特效
            //Transform fx = Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
            Transform fx = PoolManager.Pools["mypool"].Spawn("Explosion", m_transform.position, Quaternion.identity);

            m_life = 0;
            //GameManager.Instance.DelayDestoryFx(2, fx.gameObject);//2秒后销毁爆炸特效
            PoolManager.Pools["mypool"].Despawn(fx, 2);

            //Destroy(this.gameObject);//自我销毁
            Despawn();
        }
    }


    private void OnBecameInvisible()
    {
        m_isActiv = true;
    }
}
