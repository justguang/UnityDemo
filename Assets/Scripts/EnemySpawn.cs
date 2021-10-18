using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生成
/// </summary>
[AddComponentMenu("Game/EnemySpawn")]
public class EnemySpawn : MonoBehaviour
{
    public Transform m_enemyPrefab;//敌人的预制体
    public int m_enemyCount = 0;//敌人数量
    public int m_maxEnemyCount = 3;//最大敌人数量
    public float m_timer = 0;//生成敌人的时间间隔
    protected Transform m_transform;//自身的transform
    public Transform[] m_spwanPos;//敌人生成的位置

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //如果生成敌人数量达到最大值，停止生成敌人
        if (m_enemyCount >= m_maxEnemyCount) return;

        //每隔一段时间生成
        m_timer -= Time.deltaTime;
        if (m_timer <= 0)
        {
            m_timer = Random.value * 6f;//随机x秒后生成
            Transform tempSpawnPso = m_spwanPos[(int)m_timer];//获取随机的位置

            //开始生成敌人
            Transform obj = Instantiate(m_enemyPrefab, tempSpawnPso.position, Quaternion.identity);
            obj.tag = "enemy";//指定tag标签=》enemy

            //获取敌人脚本
            Enemy enemy = obj.GetComponent<Enemy>();

            //初始化敌人
            enemy.Init(this);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "item.png", true);
    }
}
