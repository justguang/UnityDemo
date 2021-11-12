using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 菜单
/// </summary>
public class Menu : MonoBehaviour
{
    //功能按钮
    public Button m_continue;//继续游戏
    public Button m_onceAgin;//重新游戏、再试一次
    public Button m_returnStart;//返回主界面
    public Button m_quitGame;//退出游戏


    // Start is called before the first frame update
    void Start()
    {
        m_continue.onClick.AddListener(delegate () { gameObject.SetActive(false); });

        m_onceAgin.onClick.AddListener(delegate () { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });

        m_returnStart.onClick.AddListener(delegate () { Sceneloading.Instance.LoadScene(0); });

        m_quitGame.onClick.AddListener(delegate () { Application.Quit(0); });
    }



}
