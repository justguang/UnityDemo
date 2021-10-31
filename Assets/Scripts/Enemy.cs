using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 敌人=》野猪
/// </summary>
public class Enemy : MonoBehaviour
{
    public Transform m_transform;

    public PathNode m_currentNode;//敌人当前所在路点
    public float m_life;//敌人生命
    public float m_maxLife;//敌人最大生命
    public float m_speed;//敌人的移速
    public int m_price;//敌人被击杀奖励的金币数
    public Action<Enemy, Transform> onDeath;//敌人死亡事件
    public Transform m_lifeBarObj;//血条
    Slider m_lifeBarSlider;//控制生命显示的血条slider

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(WaveData data)
    {
        m_transform = this.transform;

        m_life = data.baseHp;
        m_maxLife = m_life;
        m_price = data.price;
        m_speed = data.baseSpeed;

        //设置血条
        if (m_lifeBarObj == null)
            m_lifeBarObj = m_transform.Find("EnemyLifeBar");
        m_lifeBarObj.localPosition = new Vector3(0, 1.7f, 0.4f);//将血条位置调整至自身头上
        m_lifeBarObj.localScale = new Vector3(0.02f, 0.02f, 0.02f);//缩放调整血条大小
        m_lifeBarSlider = m_lifeBarObj.GetComponentInChildren<Slider>();//获取控制血条的slider
        m_lifeBarSlider.value = 1;

        StartCoroutine(UpdateLifeBar());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_currentNode == null) return;
        RotateTo();
        MoveTo();
    }

    /// <summary>
    /// 跟新血条值、位置和角度
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateLifeBar()
    {
        while (this.gameObject.activeSelf && m_lifeBarObj != null)
        {
            //更新血条值
            m_lifeBarSlider.value = (float)m_life / (float)m_maxLife;
            //更新角度，始终面向相机
            m_lifeBarObj.transform.eulerAngles = GameCamera.Instance.m_transform.eulerAngles;
            yield return 0;

        }
    }


    /// <summary>
    /// 转向目标
    /// </summary>
    public void RotateTo()
    {
        Vector3 pos = m_currentNode.m_transform.position - m_transform.position;
        pos.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(pos);//获取目标旋转角度
        float next = Mathf.MoveTowardsAngle(m_transform.eulerAngles.y, targetRotation.eulerAngles.y, 120 * Time.deltaTime);

        m_transform.eulerAngles = new Vector3(0, next, 0);
    }


    /// <summary>
    /// 向目标移动
    /// </summary>
    public void MoveTo()
    {
        Vector3 pos1 = m_transform.position;
        Vector3 pos2 = m_currentNode.m_transform.position;
        float dist = Vector2.Distance(new Vector2(pos1.x, pos1.z), new Vector2(pos2.x, pos2.z));
        if (dist < 1.0f)
        {
            //到达路点的位置
            if (m_currentNode.m_next == null)
            {
                //没有下一个路点了，说明已到达我方基地
                GameManager.Instance.ShowMsg("生命 -1", Color.red);
                GameManager.Instance.SetDamage(1);//扣除我方一点伤害
                DestroySelf();//销毁自身
            }
            else
                m_currentNode = m_currentNode.m_next;//更新到下一路点
        }

        m_transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
    }

    public void SetDamage(float damage)
    {
        m_life -= damage;
        if (m_life <= 0f)
        {
            m_life = 0f;
            //敌人死亡,增加铜钱
            GameManager.Instance.ShowMsg("铜钱 +" + m_price, Color.yellow);
            GameManager.Instance.SetPoint(m_price);
            DestroySelf();
        }
    }


    /// <summary>
    /// 销毁自身
    /// </summary>
    public void DestroySelf()
    {
        m_currentNode = null;//清空当前敌人的当前路点
        onDeath(this, m_transform);//敌人死亡事件触发
    }
}
