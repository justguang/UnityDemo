using PathologicalGames;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Action<Enemy> onAttack;//当攻击到敌人时的回调

    GameObject m_target;//目标

    Bounds m_targetCenter;//目标对象模型的边框

    Transform m_transform;

    /// <summary>
    /// 创建弓箭攻击
    /// </summary>
    /// <param name="target">攻击的目标</param>
    /// <param name="spawnPos">弓箭生成点</param>
    /// <param name="onAttack">当弓箭攻击到目标时执行</param>
    public static void Create(GameObject target, Vector3 spawnPos, Action<Enemy> onAttack)
    {
        GameObject go = PoolManager.Pools["GamePool"].Spawn("arrow", spawnPos, Quaternion.LookRotation(target.transform.position - spawnPos)).gameObject;//生成弓箭


        Projectile arrow = go.GetComponent<Projectile>();//获取弓箭添脚本
        arrow.m_target = target;//设置弓箭的弓箭目标
        arrow.m_transform = go.transform;
        //获取目标模型边框
        arrow.m_targetCenter = target.GetComponentInChildren<SkinnedMeshRenderer>().bounds;
        arrow.onAttack = onAttack;
    }


    // Update is called once per frame
    void Update()
    {
        //瞄准目标中心位置
        if (m_target != null) m_transform.LookAt(m_targetCenter.center);

        //向目标前进
        m_transform.Translate(new Vector3(0, 0, 12 * Time.deltaTime));


        if (m_target != null && m_target.activeSelf)
        {
            //通过距离检测是否打到目标
            if (Vector3.Distance(m_transform.position, m_targetCenter.center) < 0.5f)
            {
                //通知弓箭发射者打到目标
                onAttack(m_target.GetComponent<Enemy>());
                //销毁
                m_target = null;

                PoolManager.Pools["GamePool"].Despawn(m_transform);
            }
        }
        else
        {
            m_target = null;

            PoolManager.Pools["GamePool"].Despawn(m_transform);
        }

    }
}
