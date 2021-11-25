using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sceneloading : MonoBehaviour
{
    public static Sceneloading Instance;
    public Image m_bg;//背景图
    public Text m_msg;//提示消息

    public Transform m_askByQuit;//退出询问
    public Button m_quit;//退出
    public Button m_cancel;//取消

    public bool IsInfinite = false;//无限【true开启无限模式，怪物不断出现；false关闭无限】

    public int lastLevel;//玩家解锁最新关卡
    public int currLevel;//当前关卡

    string[] msgArr =
    {
        "战士攻击范围小、攻击高、攻速慢。",
        "弓箭手攻击范围大、攻击低、攻速快。",
        "只有通关最新关卡记录，才能解锁下一关卡",
        "每击杀一个敌人，会获得不同数量的金币奖励",
    };

    void Start()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }

        LoadRecord();
        Instance = this;

        //添加事件
        SceneManager.sceneLoaded += LoadSceneEnd;//场景加载事件
        Application.quitting += SaveRecord;//程序退出事件
        m_quit.onClick.AddListener(delegate () { Application.Quit(); });//UI按钮点击事件
        m_cancel.onClick.AddListener(delegate () { AskByQuit(false); });//UI按钮点击事件

        DontDestroyOnLoad(this.gameObject);

        LoadIn();
        AskByQuit(false);

        Debug.Log("SceneLoading finished.");
    }

    #region 场景淡入淡出效果
    /// <summary>
    /// 场景退出，淡出
    /// </summary>
    void LoadOut()
    {
        m_bg.raycastTarget = true;
        m_msg.text = msgArr[Random.Range(0, msgArr.Length)];
        StopCoroutine("DoLoadFade");
        StartCoroutine("DoLoadFade", 1);
    }

    /// <summary>
    /// 场景加载
    /// </summary>
    void LoadIn()
    {
        m_bg.raycastTarget = false;
        StopCoroutine("DoLoadFade");
        StartCoroutine("DoLoadFade", 0);
    }

    IEnumerator DoLoadFade(float fade)
    {
        yield return 0;
        fade = Mathf.Clamp(fade, 0, 1);
        m_bg.CrossFadeAlpha(fade, 1f, false);
        m_msg.CrossFadeAlpha(fade, 1f, false);
    }

    #endregion


    #region 加载指定场景
    public void LoadScene(int sceneIndex)
    {
        StopCoroutine("DoLoadScene");
        StartCoroutine("DoLoadScene", sceneIndex);
    }

    IEnumerator DoLoadScene(int index)
    {
        LoadOut();
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(index);
        while (!asyncOp.isDone)
        {
            yield return 0;
        }

        System.GC.Collect(0);
        LoadIn();

    }

    /// <summary>
    /// 场景加载完成
    /// </summary>
    /// <param name="scene">场景</param>
    /// <param name="loadSceneMode">加载模式</param>
    void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
#if UNITY_EDITOR
        Debug.LogError($"加载的场景名= {scene.name} 场景index={scene.buildIndex},  加载的mode= {loadSceneMode}");
#endif
    }
    #endregion


    /// <summary>
    /// 当前关卡已通关
    /// </summary>
    public void PassCurrLevel()
    {
        //如果通关的是最新记录关卡，则解锁下一关为最新关卡
        if (currLevel >= lastLevel)
        {
            lastLevel++;
            currLevel = lastLevel;
            SaveRecord();
        }
    }

    #region 游戏读、存档
    public void SaveRecord()
    {
        //存档
        if (lastLevel < currLevel) lastLevel = currLevel;

        GameFile.WriteRecord(lastLevel.ToString());
    }
    public void LoadRecord()
    {
        //读档
        lastLevel = int.Parse(GameFile.ReadRecord());
    }
    #endregion

    /// <summary>
    /// 程序退出询问
    /// </summary>
    /// <param name="isQuit"></param>
    public void AskByQuit(bool isQuit)
    {
        m_askByQuit.gameObject.SetActive(isQuit);
    }


    private void OnApplicationQuit()
    {
        Instance = null;
        System.GC.Collect(0);
    }
}
