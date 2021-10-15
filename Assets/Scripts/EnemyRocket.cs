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
            Despawn();
        }
    }

    /// <summary>
    /// unity func：当该物体在任何相机上都不可见时调用 OnBecameInvisible
    /// </summary>
    private void OnBecameInvisible()
    {
        //当离开屏幕后，如果任处于激活状态，就销毁
        //if (this.enabled)
        //Destroy(this.gameObject);

        Despawn();
    }
}
