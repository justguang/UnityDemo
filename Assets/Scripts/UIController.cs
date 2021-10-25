using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public Transform m_gameControllerUI;//游戏控制UI

    public Text m_zombieNum;//僵尸数量

    #region 血
    public Image m_bloodImage;
    public Text m_bloodTxt;
    #endregion

    #region 子弹
    public Image m_bulletImage;
    public Text m_bulletTxt;
    #endregion


    #region 弹匣
    public Image m_bulletBoxImage;
    public Text m_bulleBoxTxt;
    #endregion

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        m_gameControllerUI.gameObject.SetActive(false);
    }


    /// <summary>
    /// 设置当前血
    /// </summary>
    /// <param name="hp"></param>
    public void SetHP(int hp)
    {
        if (hp < 0) hp = 0;

        if (hp >= 60)
        {
            //健康血量
            m_bloodImage.color = Color.white;
            m_bloodTxt.color = Color.white;
        }
        else if (hp > 30 && hp < 60)
        {
            //亚健康血量
            m_bloodImage.color = Color.yellow;
            m_bloodTxt.color = Color.yellow;
        }
        else if (hp < 30)
        {
            //濒死血量
            m_bloodImage.color = Color.red;
            m_bloodTxt.color = Color.red;
        }

        m_bloodTxt.text = hp.ToString();
    }


    /// <summary>
    /// 设置子弹
    /// </summary>
    /// <param name="currentNunm">当前子弹</param>
    /// <param name="maxNum">当前子弹容量</param>
    public void SetBullet(int currentNunm, int maxNum)
    {
        if (currentNunm < 0) currentNunm = 0;
        if (currentNunm > maxNum) currentNunm = maxNum;

        int warning = (int)(maxNum * 0.3);

        if (currentNunm >= warning)
        {
            m_bulletImage.color = Color.white;
            m_bulletTxt.color = Color.white;
        }
        else if (currentNunm >= 0 && currentNunm < warning)
        {
            m_bulletImage.color = Color.yellow;
            m_bulletTxt.color = Color.yellow;
        }

        m_bulletTxt.text = currentNunm + "/" + maxNum;
    }


    /// <summary>
    /// 设置弹匣剩余子弹
    /// </summary>
    /// <param name="bulletNum">剩余子弹</param>
    public void SetBulletBox(int bulletNum)
    {
        if (bulletNum < 0) bulletNum = 0;

        m_bulleBoxTxt.text = bulletNum.ToString();
    }


    /// <summary>
    /// 更新僵尸数量UI显示
    /// </summary>
    /// <param name="curr">当前僵尸数</param>
    /// <param name="max">最大僵尸数</param>
    public void UpdateZombieNum(int curr, int max)
    {
        if (curr <= 0) curr = 0;
        m_zombieNum.text = $"僵尸数量：{curr.ToString()}";
    }

    private void OnDestroy()
    {
        Instance = null;
        System.GC.Collect();
    }
}
