using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏配置 、 参数设置
/// </summary>
public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance;

    public bool Infinity = false;//true=无限打僵尸
    public int currDifficulty = 1;//当前难度


    #region 敌人相关数据
    [Header("僵尸的数据")]
    public int maxEnemyNum;//最大敌人数量
    public float enemyHp;//敌人血量
    public float enemyATK;//敌人攻击力
    public float enemyWalkSpeed;//敌人行走速度
    public float enemyRunSpeed;//敌人奔跑速度
    public float enemyEnmityTime;//敌人仇恨值【敌人检测到玩家，追逐玩家的时长】
    public float enemyMaxEnmityTime;//敌人最大仇恨值【敌人被攻击，追逐玩家的时长】
    #endregion


    #region 玩家相关数据  
    [Header("玩家的数据")]
    public float playerATK;//玩家攻击力
    public int playerHp;//玩家血量
    public int maxStandbyBulletNum;//最大备用子弹数量
    public int maxBulletNum;//每个弹匣最大子弹数量
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
            currDifficulty = 1;
        }
        RefleshDifficulty();
    }

    /// <summary>
    /// 难度 1、2、3
    /// </summary>
    public void RefleshDifficulty()
    {
        #region 敌人数据
        maxEnemyNum = 20 * currDifficulty;
        enemyHp = 40 + (currDifficulty - 1) * 30;
        enemyATK = 2f * currDifficulty;
        enemyWalkSpeed = 0.3f;
        enemyRunSpeed = 3f + (currDifficulty - 1) * 1f;
        enemyEnmityTime = 8 + (currDifficulty - 1) * 2;
        enemyMaxEnmityTime = 16 + (currDifficulty - 1) * 3;
        #endregion

        #region 玩家数据
        playerHp = 100;
        playerATK = 5f + (currDifficulty - 1) * 2f;
        maxBulletNum = 30;
        maxStandbyBulletNum = 300 * currDifficulty;
        #endregion
    }



    private void OnDestroy()
    {
        System.GC.Collect();
    }
}
