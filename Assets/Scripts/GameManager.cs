using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[AddComponentMenu("Game/GameManager")]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    public static int m_hiscore = 0;//游戏最高得分
    public int m_score = 0;//游戏得分
    public int m_ammo = 100;//弹药数量
    Player m_player;//主角

    //UI
    Text txt_hiscore;//最高得分
    Text txt_score;//得分
    Text txt_ammo;//弹药
    Text txt_life;//生命
    Button btn_restart;//重新开始游戏

    public AudioClip m_bgm;//背景音乐
    AudioSource m_audioSource;//音源组件

    public bool isPause;//是否暂停游戏


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        //获得主角
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_audioSource = GetComponent<AudioSource>();//获取音源组件
        //获得UI文字
        GameObject canvas = GameObject.Find("Canvas");
        foreach (Transform item in canvas.transform.GetComponentsInChildren<Transform>())
        {
            if (item.name.Equals("Score"))
            {
                txt_hiscore = item.Find("Text_h").GetComponent<Text>();
                txt_score = item.Find("Text").GetComponent<Text>();
            }
            else if (item.name.Equals("Ammo"))
            {
                txt_ammo = item.Find("Text").GetComponent<Text>();
            }
            else if (item.name.Equals("Heath"))
            {
                txt_life = item.Find("Text").GetComponent<Text>();
            }
            else if (item.name.Equals("Btn_Restart"))
            {
                btn_restart = item.GetComponent<Button>();
                //设置重新开始游戏按钮事件
                btn_restart.onClick.AddListener(delegate ()
                {
                    //读取当前关卡
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
                btn_restart.gameObject.SetActive(false);//开始隐藏按钮
            }
        }
        isPause = false;
    }


    // Update is called once per frame
    void Update()
    {

        //如果按下Esc，暂停或继续游戏
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            isPause = !isPause;
            PauseGame(isPause);
        }
    }


    /// <summary>
    /// 增加得分
    /// </summary>
    /// <param name="score"></param>
    public void AddScore(int score)
    {
        m_score += score;
        if (m_score > m_hiscore) m_hiscore = m_score;//更新最高得分

        txt_score.text = $"本局得分：{m_score}";
        txt_hiscore.text = $"最高得分：{m_hiscore}";
    }


    /// <summary>
    /// 减少弹药
    /// </summary>
    /// <param name="ammo"></param>
    public void ReduceAmmo(int ammo)
    {
        m_ammo -= ammo;
        //如果弹药为负数，重新填装弹药
        if (m_ammo <= 0)
        {
            m_ammo = 100 - m_ammo;
        }

        txt_ammo.text = m_ammo.ToString() + "/100";
    }

    /// <summary>
    /// 更新生命值
    /// </summary>
    /// <param name="life"></param>
    public void SetLife(int life)
    {

        txt_life.text = life.ToString();
        //当生命值为0时，显示“重新开始游戏”按钮
        if (life <= 0) btn_restart.gameObject.SetActive(true);
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame(bool isPause)
    {
        if (isPause)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

        }
        btn_restart.gameObject.SetActive(isPause);
    }
}
