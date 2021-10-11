using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生成器
/// </summary>
[AddComponentMenu("MyScript/EnemySpawn")]
public class EnemySpawn : MonoBehaviour
{
    //public Transform m_enemyPrefab;//敌人的prefab
    Transform m_transform;
    string enemyName = "";
    bool isSuper;


    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        isSuper = gameObject.name.Contains("Super");
        if (isSuper)
        {
            enemyName = "SuperEnemy2";
        }
        else
        {
            enemyName = "Enemy";
        }
        StartCoroutine(SpawnEnemy());//执行协程函数
    }

    /// <summary>
    /// 协程创建敌人
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            int seconds = isSuper ? Random.Range(5, 13) : Random.Range(1, 7);
            //循环创建
            yield return new WaitForSeconds(seconds);//随机等待

            //Instantiate(m_enemyPrefab, m_transform.position, Quaternion.identity);//生成敌人实例
            //利用对象池生成敌人
            PoolManager.Pools["mypool"].Spawn(enemyName, m_transform.position, Quaternion.identity);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "item.png", true);
    }
}
