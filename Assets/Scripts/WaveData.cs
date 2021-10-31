using UnityEngine;

/// <summary>
/// 波数数据
/// </summary>
[System.Serializable]
public class WaveData
{
    /// <summary>
    /// 第几波，waveID
    /// </summary>
    [Header("ID第几波")]
    public int waveID = 0;

    /// <summary>
    /// 当前波次全部敌人名字
    /// </summary>
    [Header("当前波次全部敌人名字")]
    public string[] enemyArr;

    /// <summary>
    /// 当前波次所有敌人等级
    /// </summary>
    [Header("当前波次所有敌人等级")]
    public int level = 1;

    /// <summary>
    /// 当前波次敌人的基础血
    /// </summary>
    [Header("当前波次敌人的基础血")]
    public float baseHp = 3;

    /// <summary>
    /// 当前波次敌人的基础移速
    /// </summary>
    [Header("当前波次敌人的基础移速")]
    public float baseSpeed = 2;

    /// <summary>
    /// 当前波次敌人被击杀后获得的金币数
    /// </summary>
    [Header("当前波次击杀敌人的奖励")]
    public int price = 5;


    /// <summary>
    /// 当前波次每个敌人生成时间间隔[单位秒]
    /// </summary>
    [Header("当前波生成敌人时间间隔[单位秒]")]
    public float interval = 3;

}
