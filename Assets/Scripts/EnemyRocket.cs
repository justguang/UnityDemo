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
            Destroy(this.gameObject);
        }
    }
}
