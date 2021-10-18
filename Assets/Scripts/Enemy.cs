using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敌人控制脚本
/// </summary>
[AddComponentMenu("Game/Enemy")]
public class Enemy : MonoBehaviour
{
    Transform m_transform;//自身的transform
    Animator m_ani;//动画组件

    Player m_player;//主角
    NavMeshAgent m_agent;//寻路组件
    float m_moveSpeed = 2.5f;//移动速速
    float m_rotSpeed = 5.0f;//旋转速度
    float m_timer = 1f;//计时器
    int m_life = 8;//生命值
    protected EnemySpawn m_spawn;//出生点

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="spawn"></param>
    public void Init(EnemySpawn spawn)
    {
        m_spawn = spawn;
        m_spawn.m_enemyCount++;//更新当前敌人数量
    }

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;//自身transform
        m_ani = GetComponent<Animator>();//获取动画组件
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();//获得主角
        m_agent = GetComponent<NavMeshAgent>();//获得训练组件
        m_agent.speed = m_moveSpeed;//设置寻路器的行走速度
        m_agent.SetDestination(m_player.transform.position);//设置寻路目标
        m_ani.speed = 2.5f;//动画播放速度
    }

    // Update is called once per frame
    void Update()
    {
        //如果主角生命值为0，什么也不做
        if (m_player.m_life <= 0) return;
        m_timer -= Time.deltaTime;//更新计时器

        //获取当前动画状态
        AnimatorStateInfo stateInfo = m_ani.GetCurrentAnimatorStateInfo(0);
        //如果当前动画处于idle并且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.idle") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("idle", false);
            if (m_timer > 0) return;//先待机一段时间

            if (Vector3.Distance(m_transform.position, m_player.m_transform.position) < 1.5f)
            {
                //如果距离主角小于1.5m，进入攻击动画
                m_agent.ResetPath();//停止寻路
                m_ani.SetBool("attack", true);
            }
            else
            {
                //重置定时器
                m_timer = 0.5f;
                //设置寻路目标点
                m_agent.SetDestination(m_player.m_transform.position);
                //进入移动动画
                m_ani.SetBool("run", true);
            }
        }

        //如果处于run状态且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.run") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("run", false);
            //每个一段时间重新定位主角位置
            if (m_timer < 0)
            {
                m_agent.SetDestination(m_player.m_transform.position);
                m_timer = 0.5f;
            }

            //如果距离主角小于1.5m，进行攻击
            if (Vector3.Distance(m_transform.position, m_player.m_transform.position) <= 1.5f)
            {
                //停止寻路
                m_agent.ResetPath();
                //进入攻击动画
                m_ani.SetBool("attack", true);
            }
        }


        //如果处于attack状态且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.attack") && !m_ani.IsInTransition(0))
        {
            //先面向主角
            RotateTo();

            m_ani.SetBool("attack", false);

            //如果动画播完，重新进入待机状态
            if (stateInfo.normalizedTime >= 1.0f)
            {
                m_ani.SetBool("idle", true);
                m_timer = 1;//重置时间待机2秒
                m_player.OnDamage(1);//主角被攻击
            }
        }

        //如果处于death状态且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.death") && !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("death", false);
            m_spawn.m_enemyCount--;//更新当前敌人数量
            //当播放完死亡动画
            if (stateInfo.normalizedTime >= 1.0)
            {
                //加分
                GameManager.Instance.AddScore(100);
                //销毁自身
                Destroy(gameObject);
            }
        }
    }


    /// <summary>
    /// 转向目标（主角）点
    /// </summary>
    void RotateTo()
    {
        //获取主角所在的方向
        Vector3 targetDir = m_player.m_transform.position - m_transform.position;
        //计算出新方向
        Vector3 newDir = Vector3.RotateTowards(m_transform.forward, targetDir, m_rotSpeed * Time.deltaTime, 0.0f);
        //转向新方向
        m_transform.rotation = Quaternion.LookRotation(newDir);
    }

    /// <summary>
    /// 被攻击
    /// </summary>
    /// <param name="damage"></param>
    public void OnDamage(int damage)
    {
        m_life -= damage;//生命值减少1
        if (m_life <= 0)
        {
            //如果敌人当前生命值小于等于0
            m_life = 0;
            //停止寻路
            m_agent.ResetPath();
            //播放死亡动画
            m_ani.SetBool("death", true);
        }
    }
}
