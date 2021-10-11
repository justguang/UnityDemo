using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人子弹
/// </summary>
[AddComponentMenu("MyScript/EnemyRocket")]
public class EnemyRocket : Rocket
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Destroy(this.gameObject);
            //利用对象池回收
            var p = PoolManager.Pools["mypool"];
            if (p.IsSpawned(m_transform))
            {
                p.Despawn(m_transform);
            }
            else
            {
                Destroy(gameObject);
                Debug.LogWarning($"Destroy销毁{this.name}");
            }
        }
    }
}
