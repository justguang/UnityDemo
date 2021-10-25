using PathologicalGames;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool GamePause = false;//游戏暂停
    public event Action GameControllerCallBack;//游戏控制事件


    [Header("僵尸数")]
    [SerializeField] public int CurrZombieNum;//当前僵尸数
    [SerializeField] public int MaxZombieNum;//最大应生成的僵尸数

    [Space]
    [Header("僵尸出生点/巡逻点")]
    public Transform[] Points;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        Init();
    }

    public void Init()
    {
        //初始化
        CurrZombieNum = 0;
        MaxZombieNum = GameConfig.Instance.maxEnemyNum;
        GamePause = false;
        UIEvent[] uiEvents = UIController.Instance.m_gameControllerUI.GetComponentsInChildren<UIEvent>();
        for (int i = 0; i < uiEvents.Length; i++)
        {
            uiEvents[i].myDelegate_OnClick -= OtherOnClick;
            uiEvents[i].myDelegate_OnClick += OtherOnClick;
        }
        Debug.Log($"战斗场景添加的{uiEvents.Length}个UI事件");

        UIController.Instance.m_gameControllerUI.gameObject.SetActive(false);

        //PlayerController
        Transform player = PoolManager.Pools["GamePool"].Spawn("PlayerController", new Vector3(0, 0.2f, 0), Quaternion.identity, transform.parent);
        player.GetComponent<PlayerController>().Init();
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            //退出键 暂停/继续游戏
            GamePause = !GamePause;
            if (GameControllerCallBack != null) GameControllerCallBack();//事件通知

            if (GamePause)
            {
                //游戏暂停，显示鼠标
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                UIController.Instance.m_gameControllerUI.gameObject.SetActive(true);
            }
            else
            {
                UIController.Instance.m_gameControllerUI.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 随机获取一个僵尸点
    /// </summary>
    /// <returns></returns>
    public Vector3 GetOnePointByRandom()
    {
        return Points[Random.Range(0, Points.Length)].position;
    }


    /// <summary>
    /// 获取一个除pos以外的僵尸点
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 GetOnePointByRandom(Vector3 pos)
    {
        Transform[] temp = Points.Where(t => { return !t.position.Equals(pos); }).ToArray();
        return temp[Random.Range(0, temp.Length)].position;
    }

    /// <summary>
    /// 生成一个僵尸
    /// </summary>
    public void AddOneForCurrZombieNum()
    {
        CurrZombieNum += 1;
        UIController.Instance.UpdateZombieNum(CurrZombieNum, MaxZombieNum);
    }

    /// <summary>
    /// 回收一个僵尸
    /// </summary>
    public void DestroyOneForCurrZombieNum()
    {
        CurrZombieNum -= 1;
        UIController.Instance.UpdateZombieNum(CurrZombieNum, MaxZombieNum);
    }


    #region 整体游戏控制

    void OtherOnClick(object obj)
    {
        GameObject tempObj = obj as GameObject;
        if (tempObj == null) return;
        UIController.Instance.m_gameControllerUI.gameObject.SetActive(false);

        switch (tempObj.name)
        {
            case "Button_Continue":
                //游戏继续
                GamePause = false;
                if (GameControllerCallBack != null) GameControllerCallBack();//事件通知
                break;
            case "Button_Restart":
                //重新游戏
                ScenesLoading.Instance.StartLoading(SceneManager.GetActiveScene(), 1);
                break;
            case "Button_ReturnMain":
                //退出主界面
                if (ScenesLoading.Instance != null)
                {
                    ScenesLoading.Instance.StartLoading(SceneManager.GetActiveScene(), 0);
                }
                else
                {
                    Application.Quit();
                }
                break;
        }
    }

    void Quit()
    {
        GamePause = true;
        if (Instance != null)
            Instance = null;
        PoolManager.Pools["GamePool"]?.DespawnAll();

    }

    /// <summary>
    /// 退出程序
    /// </summary>
    private void OnApplicationQuit()
    {
        Quit();

    }
    #endregion

    private void OnDestroy()
    {
        System.GC.Collect();
    }
}
