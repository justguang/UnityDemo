using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("MyScript/GameManager")]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//静态实例

    public Record record;//分数记录

    public Transform m_canvas_main;//显示分数的UI界面
    public Transform m_canvas_gameover;//游戏结束UI界面
    public Text m_text_score;//本局得分
    public Text m_text_best;//历史最高得分
    public Text m_text_life;//生命

    public static int m_hiscore;//历史最高分数值
    protected int m_score;//得分数值
    protected Player m_player;//主角实例

    public AudioClip m_bgmClip;//背景音乐
    protected AudioSource m_audio;//声音源


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        m_audio = this.GetComponent<AudioSource>();
        m_audio.clip = m_bgmClip;//指定背景音乐
        m_audio.loop = true;//设置声音循环播放
        m_audio.Play();//开始播放音乐

        //通过tag标签查找主角
        m_player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();

        //获取历史最高分
        if (record == null) record = m_canvas_gameover.Find("Record")?.GetComponent<Record>();
        if (record != null)
        {
            record.RefleshRecord(false);
            m_hiscore = record.maxRecord;
        }

        //获取UI控件,并初始化数值
        m_text_score = m_canvas_main.Find("Text_score")?.GetComponent<Text>();
        m_text_best = m_canvas_main.Find("Text_best")?.GetComponent<Text>();
        m_text_life = m_canvas_main.Find("Text_life")?.GetComponent<Text>();
        if (m_text_score != null) m_text_score.text = string.Format("本局分数：{0}", m_score);
        if (m_text_best != null) m_text_best.text = string.Format("历史最高分：{0}", m_hiscore);
        if (m_text_life != null && m_player != null) m_text_life.text = string.Format("生命：{0}", m_player.m_life);


        //获取重新开始按钮
        Button button_restart = m_canvas_gameover.Find("Button_restart")?.GetComponent<Button>();
        if (button_restart != null)
        {
            //设置重新开始按钮事件回调
            button_restart.onClick.AddListener(delegate ()
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);//重新开始当前关卡【当前场景】
            });
        }

        m_canvas_gameover.gameObject.SetActive(false);//默认隐藏gameoverUI界面

    }


    /// <summary>
    /// 增加分数函数
    /// </summary>
    /// <param name="point">要加多少分</param>
    public void addScore(int point)
    {
        m_score += point;

        if (m_text_score != null) m_text_score.text = string.Format("本局分数：{0}", m_score);
        //if (m_text_best != null) m_text_best.text = string.Format("历史最高分：{0}", m_hiscore);

    }

    /// <summary>
    /// 生命值改变
    /// </summary>
    /// <param name="life"></param>
    public void ChangeLife(int life)
    {
        if (m_text_life != null && life >= 0) m_text_life.text = string.Format("生命：{0}", life);
        if (life == 0)
        {
            //保存最高分
            if (record != null) record.RefleshRecord(false, m_score);


            m_canvas_gameover.gameObject.SetActive(true);//如果生命值为0，gameoverUI界面显示
        }
    }

    /// <summary>
    /// 延时销毁特效obsolete 
    /// </summary>
    [Obsolete("该方法已过时：该方法最终执行的是Destroy销毁对象，会对游戏运行的性能造成损耗，不建议使用，推荐使用PoolManager对象池")]
    public void DelayDestoryFx(int seconds, GameObject destoryObj)
    {
        StartCoroutine(DestoryFx(seconds, destoryObj));
    }

    [Obsolete("该方法已过时，建议用对象池进行回收")]
    IEnumerator DestoryFx(int time, GameObject destoryObj)
    {
        yield return new WaitForSeconds(time);
        Destroy(destoryObj);
        yield break;
    }


    /// <summary>
    /// 退出程序
    /// </summary>
    private void OnApplicationQuit()
    {
        //保存最高分
        if (record != null) record.RefleshRecord(false, m_score);
    }


}
