using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{

    [Header("声音相关")]
    public AudioSource m_audioSourceByZombie;
    public AudioClip[] m_audioClip;

    [Header("切换场景要隐藏的点")]
    public Transform[] hideByUnloadScene;


    [Header("画布")]
    public Transform m_canvas;
    public Transform m_uiPanelConfig;

    private UIEvent[] uiEvents;

    // Start is called before the first frame update
    void Start()
    {
        if (m_canvas != null)
        {
            uiEvents = m_canvas.GetComponentsInChildren<UIEvent>();
            if (uiEvents != null)
            {
                for (int i = 0; i < uiEvents.Length; i++)
                {
                    uiEvents[i].myDelegate_OnClick -= OnClick;
                    uiEvents[i].myDelegate_OnClick += OnClick;
                }
            }
            Debug.Log($"Start场景添加{uiEvents.Length}个UI事件");
        }
        else
        {
            Debug.LogError("未找到画布");
        }
        m_uiPanelConfig.gameObject.SetActive(false);
    }


    /// <summary>
    /// UI事件
    /// </summary>
    /// <param name="obj"></param>
    public void OnClick(object obj)
    {
        GameObject tempObj = obj as GameObject;
        if (tempObj == null) return;
        switch (tempObj.name)
        {
            case "MainButton_StartGame":
                //开始游戏
                StartCoroutine(DoStartGame());
                break;
            case "MainButton_Quit":
                //退出游戏
                Application.Quit();
                break;
            case "MainButton_Config":
                //打开配置
                m_uiPanelConfig.gameObject.SetActive(true);
                break;
            case "1":
            //游戏难度 1
            case "2":
            //游戏难度 2
            case "3":
                //游戏难度 3
                m_uiPanelConfig.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <returns></returns>
    IEnumerator DoStartGame()
    {
        for (int i = 0; i < hideByUnloadScene.Length; i++)
        {
            hideByUnloadScene[i].gameObject.SetActive(false);
        }
        transform.GetComponent<Animator>().Play("ScreamByStart");

        yield return new WaitForSeconds(2f);
        ScenesLoading.Instance.StartLoading(SceneManager.GetActiveScene(), 0);

        yield break;
    }


    private void OnDestroy()
    {
        System.GC.Collect();
    }


    #region 音效
    /// <summary>
    /// 僵尸咆哮
    /// </summary>
    public void ScreamAudio()
    {
        m_audioSourceByZombie.PlayOneShot(m_audioClip[1]);
    }
    #endregion
}
