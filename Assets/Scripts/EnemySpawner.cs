using PathologicalGames;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 敌人生成器
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    Transform m_transform;

    [Header("敌人起始点")]
    public PathNode m_startNode;//起始点

    private int m_liveEnemy = 0;//还存活的敌人数
    int enemyIndex = 0;//生成敌人数组的下标
    int waveIndex = 0;//战斗波数数组下标

    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        //开始生成敌人
        StartCoroutine(DoSpawnEnmeies());
    }

    private void OnDestroy()
    {
        StopCoroutine(DoSpawnEnmeies());
    }

    IEnumerator DoSpawnEnmeies()
    {
        yield return new WaitForEndOfFrame();//保证在Start函数后执行

        while (GameManager.Instance.waves.Count <= 0)
        {
            yield return 1;
        }

        int maxWave = GameManager.Instance.waves.Count;//最大波数
        enemyIndex = 0;//本波次，敌人数组下标，第几个敌人

        GameManager.Instance.ShowMsg("第 " + (waveIndex + 1) + " 波敌人来袭", Color.yellow);
        GameManager.Instance.SetWave(waveIndex + 1);//设置UI上的波数显示

        WaveData wave = GameManager.Instance.waves[waveIndex];//获取当前波数数据
        yield return new WaitForSeconds(wave.interval);//生成敌人时间间隔

        int waveCount = wave.enemyArr.Length;
        while (enemyIndex < waveCount)
        {
            Vector3 dir = m_startNode.m_transform.position - m_transform.position;//初始方向
            //实例化敌人
            Transform enemyTrans = PoolManager.Pools["GamePool"].Spawn(wave.enemyArr[enemyIndex], m_transform.position, Quaternion.LookRotation(dir), m_transform);

            Enemy enemy = enemyTrans.GetComponent<Enemy>();//获得敌人脚本
            enemy.m_currentNode = m_startNode;//设置敌人的第一个路点
            enemy.Init(wave);//初始化


            GameManager.Instance.m_enemyList.Add(enemy);
            m_liveEnemy++;//增加敌人数量

            //当敌人死亡时减少敌人数量
            enemy.onDeath = new Action<Enemy, Transform>((Enemy e, Transform tran) =>
             {
                 m_liveEnemy--;
                 GameManager.Instance.m_enemyList.Remove(e);//战场存活的敌人-1
                 PoolManager.Pools["GamePool"].Despawn(tran);
             });

            enemyIndex++;//更新敌人数组下标

            yield return new WaitForSeconds(wave.interval);//生成敌人时间间隔
        }

        //创建完全部敌人，等待全部被消灭
        while (m_liveEnemy > 0)
        {
            yield return 0;
        }

        enemyIndex = 0;//重置敌人数组下标
        waveIndex++;//更新战斗波数
        if (waveIndex < GameManager.Instance.waves.Count)
        {
            //如果还有波数
            StartCoroutine(DoSpawnEnmeies());//继续生成后面的敌人
        }
        else
        {
            //通知胜利
            GameManager.Instance.ShowMsg("恭喜通关！！", Color.green);
            Debug.LogError("胜利");
            GameManager.Instance.RestartGame();
        }
    }

    /// <summary>
    /// 在编辑器中显示一个图标
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "spawner.tif");
    }
}
