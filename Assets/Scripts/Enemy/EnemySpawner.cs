using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生成器
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    Transform m_transform;//自身的transform

    List<WaveData> waves;//当前关卡所有敌人

    [Header("敌人起始点")]
    public PathNode m_startNode;//起始点

    private int m_liveEnemy = 0;//还存活的敌人数
    int enemyIndex = 0;//生成敌人数组的下标
    int waveIndex = 0;//战斗波数数组下标

    public bool isInfinite = false;//无限模式
    public int infiniteEnemyWaveIndex = 0;//无限模式下的波数

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        m_transform = this.transform;
    }


    private void OnApplicationQuit()
    {
        Quit();
    }

    void OnDestroy()
    {
        Quit();
    }

    void Quit()
    {
        StopCoroutine(DoSpawnEnmeies());
        if (waves != null)
            waves.Clear();
        Instance = null;
    }

    /// <summary>
    /// 开始
    /// </summary>
    /// <param name="waves"></param>
    public void StartSpawn(List<WaveData> waves)
    {
        this.waves = waves;

        if (this.waves == null || this.waves.Count <= 0)
        {
            Debug.LogError("敌人生成器中没有要生成的敌人，waves.Count==0");
            return;
        }

        //开始生成敌人
        StartCoroutine(DoSpawnEnmeies());
    }


    IEnumerator DoSpawnEnmeies()
    {
        yield return new WaitForEndOfFrame();

        enemyIndex = 0;//本波次，敌人数组下标，第几个敌人
        WaveData wave = this.waves[waveIndex];//获取当前波数数据

        int amountWave = waveIndex + 1 + infiniteEnemyWaveIndex;

        GameManager.Instance.ShowMsg("第 " + amountWave + " 波敌人来袭", Color.yellow);
        GameManager.Instance.SetWave(amountWave);//设置UI上的波数显示

        if (isInfinite)
            yield return new WaitForSeconds(1.5f);//无限模式下生成敌人时间间隔
        else
            yield return new WaitForSeconds(wave.interval);//生成敌人时间间隔


        int enemyCount = wave.enemyArr.Length;
        while (enemyIndex < enemyCount)
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

            if (isInfinite)
                yield return new WaitForSeconds(1.5f);//无限模式下生成敌人时间间隔
            else
                yield return new WaitForSeconds(wave.interval);//生成敌人时间间隔
        }



        //创建完全部敌人，等待全部被消灭【无限模式下跳过等待】
        while (!isInfinite && m_liveEnemy > 0)
        {
            yield return 0;
        }
        yield return 0;

        enemyIndex = 0;//重置敌人数组下标
        waveIndex++;//更新战斗波数
        if (waveIndex < this.waves.Count)
        {
            //如果还有波数
            StartCoroutine(DoSpawnEnmeies());//继续生成后面的敌人
        }
        else
        {
            if (isInfinite)
            {
                //如果时无限模式，重置波数下标，继续生成敌人
                waveIndex = 0;
                infiniteEnemyWaveIndex += this.waves.Count;//更新无限模式下的波数
                StartCoroutine(DoSpawnEnmeies());
            }
            else
            {
                //通知胜利
                Debug.LogError("胜利");
                this.waves.Clear();
                this.m_startNode = null;
                GameManager.Instance.PassCurrLevel();
            }
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
