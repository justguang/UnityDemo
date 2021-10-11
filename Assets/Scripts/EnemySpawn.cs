using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生成器
/// </summary>
[AddComponentMenu("MyScript/EnemySpawn")]
public class EnemySpawn : MonoBehaviour
{
    public Transform m_enemyPrefab;//敌人的prefab
    Transform m_transform;



    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        StartCoroutine(SpawnEnemy());//执行协程函数
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 协程创建敌人
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            //循环创建
            yield return new WaitForSeconds(Random.Range(5, 15));//随机等待5-15秒
            Instantiate(m_enemyPrefab, m_transform.position, Quaternion.identity);//生成敌人实例
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "item.png",true);
    }
}
