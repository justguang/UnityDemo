using System.Collections;
using UnityEngine;

/// <summary>
/// 远程防守单位
/// </summary>
public class ArcherDefender : Defender
{

    protected override IEnumerator DoAttack()
    {
        //没有可攻击的敌人就一致等待
        while (m_targetEnemy == null || !m_isFaceEnemy || !m_targetEnemy.gameObject.activeSelf)
        {
            m_targetEnemy = null;
            yield return 0;
        }

        m_isAttacked = false;
        m_ani.Play("attack");//播放攻击动画

        //等待进入攻击动画
        while (!m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            yield return 0;
        }

        float ani_lenght = m_ani.GetCurrentAnimatorStateInfo(0).length;//获取攻击动画的长度
        if (m_targetEnemy.gameObject.activeSelf && CheckEnemy(m_targetEnemy.m_transform.position) <= (m_config.attackArea + 0.5f))
        {
            yield return new WaitForSeconds(ani_lenght * 0.5f);//等待完成攻击动作
            Vector3 pos = this.m_model.transform.Find("atkpoint").position;//获取攻击点的位置

            if (m_targetEnemy.gameObject.activeSelf)
            {
                //创建弓箭
                Projectile.Create(m_targetEnemy.gameObject, pos, (Enemy e) =>
                {
                    if (m_targetEnemy != null && m_targetEnemy.gameObject.activeSelf)
                        e.SetDamage(m_config.power);
                });
                m_isAttacked = true;//已经攻击过了
            }
            else
                m_ani.Play("idle");//播放待机动画

        }
        else
        {
            m_ani.Play("idle");
        }
        m_targetEnemy = null;

        //已经攻击过，等待攻击时间间隔
        if (m_ani.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            yield return new WaitForSeconds(ani_lenght * 0.4f);//播放剩余的攻击动画
            m_ani.Play("idle");//播放待机动画
        }

        //如果已经攻击过就等待时间间隔
        if (m_isAttacked)
            yield return new WaitForSeconds(m_config.attackInterval);//攻击时间间隔

        StartCoroutine(DoAttack());//下一轮攻击
    }
}
