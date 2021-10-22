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
    [SerializeField] private int CurrZombieNum;//当前僵尸数
    [SerializeField] private int MaxZombieNum;//最大应生成的僵尸数
    public bool CanGenerateZombie { get { return CurrZombieNum < MaxZombieNum; } }//true=》可以生成僵尸

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
        MaxZombieNum = 20;
        GamePause = false;
        UIController.Instance.m_gameControllerUI.gameObject.SetActive(false);
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

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        GamePause = false;
        if (GameControllerCallBack != null) GameControllerCallBack();//事件通知
        UIController.Instance.m_gameControllerUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 重新游戏
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);//重新开始游戏
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    private void OnApplicationQuit()
    {
        GamePause = true;
        PoolManager.Pools["GamePool"].DespawnAll();
        System.GC.Collect();
    }

    #endregion
}
