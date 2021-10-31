using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子状态
/// </summary>
public enum TileSatus
{
    DEAD = 0,//禁止【不能在改格子上做任何事】
    ROAD = 1,//专用于敌人行走的格子
    GUARD = 2,//专用于创建防守单位的格子
}


public class Defender : MonoBehaviour
{
    public DefenderConfig m_config;//改防守单位的配置

    protected Enemy m_targetEnemy;//目标敌人
    protected bool m_isFaceEnemy;//是否已经面向敌人
    protected bool m_isAttacked;//是否已经攻击过
    protected GameObject m_model;//模型
    protected Animator m_ani;//动画
    protected Transform m_transform;//自身的transform

    protected void Start()
    {
        m_transform = transform;
    }

    /// <summary>
    /// 创建防守单位
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pos"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static T Create<T>(DefenderConfig config, Vector3 pos, Vector3 angle) where T : Defender
    {
        GameObject go = new GameObject("defender");
        go.transform.position = pos;
        go.transform.eulerAngles = angle;
        T d = go.AddComponent<T>();
        d.Init(config);

        //将自己占的格子信息设为占用
        TileObject.Instance.SetDataFromPosition(d.transform.position.x, d.transform.position.z, (int)TileSatus.DEAD);

        return d;
    }


    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Init(DefenderConfig config)
    {
        m_config = config;
        m_isAttacked = false;

        //创建模型
        CreateModel(config.name);
        StartCoroutine(DoAttack());//执行攻击逻辑
    }

    /// <summary>
    /// 创建模型
    /// </summary>
    /// <param name="modelName"></param>
    protected virtual void CreateModel(string modelName)
    {
        m_model = PoolManager.Pools["GamePool"].Spawn(modelName, transform.position, transform.rotation, transform).gameObject;
        m_ani = m_model.GetComponent<Animator>();
    }

    protected void Update()
    {
        FindEnemy();
        RotateTo();
        //DoAttack();
    }

    /// <summary>
    /// 面向敌人
    /// </summary>
    public void RotateTo()
    {
        if (m_targetEnemy == null) return;

        Vector3 targetDir = m_targetEnemy.m_transform.position - m_transform.position;
        targetDir.y = 0;

        //获取旋转方向
        Vector3 rot_delta = Vector3.RotateTowards(m_transform.forward, targetDir, 20.0f * Time.deltaTime, 0.0f);
        Quaternion targetRotation = Quaternion.LookRotation(rot_delta);

        //计算当前方向与目标的角度
        float angle = Vector3.Angle(targetDir, m_transform.forward);
        if (angle < 1.0f)
        {
            //如果已经面向敌人
            m_isFaceEnemy = true;
        }
        else
        {
            m_isFaceEnemy = false;
        }

        m_transform.rotation = targetRotation;
    }

    /// <summary>
    /// 查找敌人
    /// </summary>
    protected void FindEnemy()
    {
        if (m_targetEnemy != null) return;
        m_targetEnemy = null;
        float minlife = 0;//最低生命值

        List<Enemy> enemies = GameManager.Instance.m_enemyList;
        int enmiesCount = enemies.Count;
        for (int i = 0; i < enmiesCount; i++)
        {
            //遍历所有敌人，生命值无的跳过
            if (enemies[i].m_life <= 0) continue;

            //计算与敌人的距离
            if ((CheckEnemy(enemies[i].m_transform.position) - 0.5f) > m_config.attackArea) continue;//跳过不在攻击范围的敌人

            //查找最低生命值的敌人
            if (minlife <= 0 || minlife > enemies[i].m_life)
            {
                m_targetEnemy = enemies[i];
                minlife = enemies[i].m_life;
            }
        }
    }

    /// <summary>
    /// 检测敌人与防守单位的距离
    /// </summary>
    /// <param name="enemyPos"></param>
    /// <returns></returns>
    protected float CheckEnemy(Vector3 enemyPos)
    {
        Vector3 mPos = m_transform.position;
        mPos.y = 0;
        enemyPos.y = 0;
        return Vector3.Distance(mPos, enemyPos);
    }

    /// <summary>
    /// 攻击
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator DoAttack()
    {
        //如果没有找到敌人 就一直等待
        while (m_targetEnemy == null || !m_isFaceEnemy || !m_targetEnemy.gameObject.activeSelf)
        {
            m_targetEnemy = null;
            yield return 0;
        }

        m_isAttacked = false;
        m_ani.Play("attack"); //播放攻击动画

        //等待进入攻击动画
        while (!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            yield return 0;
        }

        float ani_lenght = m_ani.GetCurrentAnimatorStateInfo(0).length;
        if (m_targetEnemy.gameObject.activeSelf && CheckEnemy(m_targetEnemy.m_transform.position) <= (m_config.attackArea + 0.5f))
        {
            //获取攻击动画长度
            yield return new WaitForSeconds(ani_lenght * 0.4f);//等待完成攻击动作

            //攻击
            if (m_targetEnemy.gameObject.activeSelf)
            {
                m_targetEnemy.SetDamage(m_config.power);
                m_isAttacked = true;
            }
            else
                m_ani.Play("idle");//播放待机动画

        }
        else
        {
            m_ani.Play("idle");//播放待机动画
        }

        m_targetEnemy = null;


        //已经攻击过，等待攻击时间间隔
        if (m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            yield return new WaitForSeconds(ani_lenght * 0.5f);//播放剩余的攻击动画
            m_ani.Play("idle");//播放待机动画
        }

        if (m_isAttacked)
            yield return new WaitForSeconds(m_config.attackInterval);//攻击时间间隔


        StartCoroutine(DoAttack());//下一轮攻击

    }

}
