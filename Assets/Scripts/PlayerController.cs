using PathologicalGames;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/// <summary>
/// 玩家状态
/// </summary>
public enum PlayerState
{
    Idle,
    Shoot,
    Reload,
    Death,
}


public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    #region 相关组件
    private Transform m_transform;//自身transform
    private Animator m_ani;//动画控制器
    private AudioSource m_audioSource;//音源
    private FirstPersonController firstPersonController;//unity官方的第一人称控制脚本
    private RoamController m_roamController;//漫游脚本【玩家死亡，激活漫游】
    #endregion


    [SerializeField] private int hp;//玩家血量
    [SerializeField] private float ATK;//攻击力

    #region 相机【第一个是场景主相机】
    [Space]
    [SerializeField] private Camera[] cameras;
    #endregion

    #region 武器
    [Space]
    [Header("武器相关")]
    public Transform m_weapon;//武器
    public Image m_weaponCrossImage;//武器准星
    private Vector3 weaponInitPos = new Vector3(0.386f, -0.3f, 0.32f);//武器初始位置
    [SerializeField] private AudioClip[] m_audioClips;//一些音效
    #endregion

    #region 射击参数
    private bool canShoot = true;//是否可以射击【默认true可以射击】
    private float shootInterval;//射击时间间隔
    #endregion

    #region 射击效果
    [Space]
    [Header("子弹相关")]
    [Range(0, 5f)] public float m_shootRecoil;//射击后座力
    public Transform m_firePoint_FX;//射击点的火花特效
    #endregion


    #region 弹匣数据相关
    /// <summary>
    /// 当前子弹数量
    /// </summary>
    [SerializeField] private int curr_BulletNum;
    public int Curr_BulletNum
    {
        get { return curr_BulletNum; }
        set
        {
            curr_BulletNum = value;
            UIController.Instance.SetBullet(Curr_BulletNum, Max_BulletNum);
        }
    }

    /// <summary>
    /// 填充的最大子弹数量
    /// </summary>
    private int max_BulletNum;
    public int Max_BulletNum
    {
        get { return max_BulletNum; }
        set
        {
            max_BulletNum = value;
        }
    }

    /// <summary>
    /// 当前备用子弹数量
    /// </summary>
    [SerializeField] private int curr_StandbyBulletNum;
    public int Curr_StandbyBulletNum
    {
        get { return curr_StandbyBulletNum; }
        set
        {
            curr_StandbyBulletNum = value;
            UIController.Instance.SetBulletBox(Curr_StandbyBulletNum);
        }
    }

    /// <summary>
    /// 最大备用子弹数量
    /// </summary>
    private int max_StandbyBulletNum;
    public int Max_StandbyBulletNum
    {
        get { return max_StandbyBulletNum; }
        set
        {
            max_StandbyBulletNum = value;
        }
    }
    #endregion


    #region 玩家状态
    [SerializeField] private PlayerState playerState;
    public PlayerState PlayerState
    {
        get => playerState;

        //当状态改变时，进行对应的操作
        set
        {

            playerState = value;
            switch (playerState)
            {
                case PlayerState.Idle:
                    m_firePoint_FX.gameObject.SetActive(false);//关闭射击火花特效
                    m_ani.SetBool("Reload", false);
                    m_ani.SetBool("Shoot", false);
                    break;
                case PlayerState.Shoot:
                    //当前弹匣有子弹发射，没有就填装子弹
                    if (Curr_BulletNum > 0)
                    {
                        Shoot();
                    }
                    else
                    {
                        if (Curr_StandbyBulletNum > 0)
                            PlayerState = PlayerState.Reload;
                        else
                            PlayerState = PlayerState.Idle;//如果备用子弹也用完了，就进入Idle
                    }

                    break;
                case PlayerState.Reload:
                    //如果用备用子弹就换弹匣填充子弹,并且当前子弹不是满的
                    if (Curr_StandbyBulletNum > 0 && Curr_BulletNum < Max_BulletNum)
                    {
                        CancelAim();
                        m_firePoint_FX.gameObject.SetActive(false);//关闭射击火花特效
                        m_ani.SetBool("Shoot", false);
                        m_ani.SetBool("Reload", true);
                        PlayAudio(1);
                    }
                    else
                    {
                        //没有备用子弹就Idle
                        PlayerState = PlayerState.Idle;
                    }
                    break;
                case PlayerState.Death:
                    Debug.LogError("玩家死亡");
                    //激活漫游相机
                    m_roamController.transform.SetParent(transform.parent);
                    m_roamController.enabled = true;

                    //隐藏自身控制
                    gameObject.SetActive(false);
                    break;
            }

        }
    }
    #endregion

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //控制帧率
        Application.targetFrameRate = 60;

        //Init();

    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        #region 获取相关组件
        m_transform = this.transform;
        m_ani = m_transform.Find("FirstPersonCharacter/Weapon").GetComponent<Animator>();
        m_audioSource = GetComponent<AudioSource>();
        firstPersonController = GetComponent<FirstPersonController>();
        m_roamController = transform.Find("FirstPersonCharacter").GetComponent<RoamController>();
        if (m_firePoint_FX == null) { m_firePoint_FX = transform.Find("FirstPersonCharacter/Weapon/rifle/base/FirePoint_FX"); }
        cameras[0] = Camera.main;
        #endregion

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        if (m_roamController != null)
        {
            m_roamController.enabled = false;//取消激活漫游脚本
            m_roamController.transform.SetParent(transform);
            m_roamController.transform.localPosition = new Vector3(0, 1, 0);
            m_roamController.transform.localEulerAngles = Vector3.zero;
        }
        hp = GameConfig.Instance.playerHp;
        ATK = GameConfig.Instance.playerATK;
        BeAttack(0);
        weaponInitPos = new Vector3(0.386f, -0.3f, 0.32f);//武器初始位置

        //初始化弹匣相关
        Max_StandbyBulletNum = GameConfig.Instance.maxStandbyBulletNum;//备用子弹数
        Curr_StandbyBulletNum = Max_StandbyBulletNum;
        Max_BulletNum = GameConfig.Instance.Infinity ? 99999 : GameConfig.Instance.maxBulletNum;//子弹最大填充数
        Curr_BulletNum = Max_BulletNum;

        canShoot = true;//启用射击
        shootInterval = 0.14f;//射击时间间隔
        m_shootRecoil = 3f;//射击后座力

        //FrontSight
        m_weaponCrossImage = UIController.Instance.transform.Find("FrontSight").GetComponent<Image>();

        PlayerState = PlayerState.Idle;


        //订阅事件
        GameManager.Instance.GameControllerCallBack -= OnEventByGameController;
        GameManager.Instance.GameControllerCallBack += OnEventByGameController;
    }

    void Update()
    {
        if (GameManager.Instance.GamePause) return;
        if (hp <= 0)
        {
            if (PlayerState != PlayerState.Death)
            {
                //玩家死亡
                PlayerState = PlayerState.Death;
            }
            return;
        }

        StateForUpdate();
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("隐藏功能，按下P键，扣除自身血量");
            BeAttack(GameConfig.Instance.enemyATK * 2);
        }
    }


    void StateForUpdate()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                if (canShoot && Input.GetMouseButton(0))
                {
                    //鼠标左键射击
                    PlayerState = PlayerState.Shoot;
                    return;
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    //R键换弹
                    PlayerState = PlayerState.Reload;
                    return;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    //鼠标右键 瞄准
                    Aim();
                }
                if (Input.GetMouseButtonUp(1))
                {
                    //松开鼠标右键取消瞄准
                    CancelAim();
                }
                break;
            case PlayerState.Shoot:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    CancelInvoke("ReShootCD");
                    canShoot = true;//可以射击

                    //R键换弹
                    PlayerState = PlayerState.Reload;
                    return;
                }

                if (Input.GetMouseButtonUp(1))
                {
                    //松开鼠标右键取消瞄准
                    CancelAim();
                }
                break;
            case PlayerState.Reload:
                //判断当前动画clip是Reload && 动画已播放完
                if (m_ani.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Reload") && !m_ani.IsInTransition(0))

                {
                    float time = m_ani.GetCurrentAnimatorStateInfo(0).normalizedTime;

                    //判断当前动画已播放完
                    if (time >= 1f)
                    {
                        //子弹填充
                        int temp = Max_BulletNum - Curr_BulletNum;
                        if (Curr_StandbyBulletNum - temp < 0)
                        {
                            temp = Curr_StandbyBulletNum;
                        }
                        Curr_StandbyBulletNum -= temp;
                        Curr_BulletNum += temp;

                        PlayerState = PlayerState.Idle;//换弹完毕，状态改为Idle
                    }

                }
                break;
        }
    }

    #region 射击

    /// <summary>
    /// 玩家开枪射击
    /// </summary>
    void Shoot()
    {
        Curr_BulletNum -= 1;//当前子弹减少
        canShoot = false;
        m_ani.SetBool("Shoot", true);
        m_firePoint_FX.gameObject.SetActive(true);//火花特效
        StartShootRecoil();
        PlayAudio(0);

        //射线检测
        Ray ray = cameras[0].ScreenPointToRay(Input.mousePosition);
        int layer = (1 << 9) | (1 << 10);//9僵尸，10场景
        if (Physics.Raycast(ray, out RaycastHit info, 1500f, layer))
        {
            string prefabName = "";
            if (info.transform.gameObject.layer == 9)
            {
                //打到僵尸
                //Debug.LogError("打到僵尸");
                prefabName = "WFX_BImpact SoftBody + Hole";
                info.transform.GetComponent<ZombieController>().BeAttack(ATK);
            }
            else if (info.transform.gameObject.layer == 10)
            {
                //打到场景
                //Debug.LogError("打到场景");
                prefabName = "WFX_BImpact Concrete + Hole Lit";
            }

            if (!string.IsNullOrWhiteSpace(prefabName))
            {
                //实例化弹坑
                Transform obj = PoolManager.Pools["GamePool"].Spawn(prefabName, info.point, Quaternion.identity, info.transform);
                obj.LookAt(cameras[0].transform);//让弹坑面向相机
            }
        }

        Invoke("ReShootCD", shootInterval);
    }

    /// <summary>
    /// 射击CD
    /// </summary>
    void ReShootCD()
    {
        canShoot = true;
        PlayerState = PlayerState.Idle;
    }
    #endregion

    /// <summary>
    /// 播放音效0射击，1换弹匣，2子弹掉落
    /// </summary>
    /// <param name="index"></param>
    void PlayAudio(int index)
    {
        m_audioSource.PlayOneShot(m_audioClips[index]);
    }

    #region 射击后座力

    /// <summary>
    /// 开始射击后座力
    /// </summary>
    private void StartShootRecoil()
    {
        //准星
        StartCoroutine(ShootRecoilCross());

        //相机
        StartCoroutine(ShootRecoilCamera());
    }

    /// <summary>
    /// 射击后座力的准星协程
    /// </summary>
    /// <returns></returns>
    IEnumerator ShootRecoilCross()
    {
        Vector2 scale = m_weaponCrossImage.transform.localScale;
        //放大
        while (scale.x <= 1.5f)
        {
            yield return null;
            scale.x += m_shootRecoil * Time.deltaTime;
            scale.y += m_shootRecoil * Time.deltaTime;

            m_weaponCrossImage.transform.localScale = scale;
        }

        //缩小
        while (scale.x > 1)
        {
            yield return null;
            scale.x -= Time.deltaTime;
            scale.y -= Time.deltaTime;

            m_weaponCrossImage.transform.localScale = scale;
        }
        scale = Vector2.one;
        m_weaponCrossImage.transform.localScale = scale;
    }

    /// <summary>
    /// 射击后座力的相机协程
    /// </summary>
    /// <returns></returns>
    IEnumerator ShootRecoilCamera()
    {
        float xOffset = Random.Range(0.3f, 0.6f);
        float yOffset = Random.Range(-0.15f, 0.15f);

        firstPersonController.xRotOffset = xOffset;
        firstPersonController.yRotOffset = yOffset;
        yield return 10;


        firstPersonController.xRotOffset = -xOffset;
        firstPersonController.yRotOffset = -yOffset;
        yield return 10;

        firstPersonController.xRotOffset = 0;
        firstPersonController.yRotOffset = 0;

    }
    #endregion


    #region 鼠标右键准星瞄准
    /// <summary>
    /// 瞄准
    /// </summary>
    private void Aim()
    {
        StopCoroutine("DoCancelAim");
        StopCoroutine("DoAim");
        StartCoroutine("DoAim");
    }

    /// <summary>
    /// 取消瞄准
    /// </summary>
    private void CancelAim()
    {
        StopCoroutine("DoAim");
        StopCoroutine("DoCancelAim");
        StartCoroutine("DoCancAim");
    }

    IEnumerator DoAim()
    {
        //瞄准
        //先改变武器位置
        Vector3 pos = m_weapon.localPosition;
        while (pos.x > 0)
        {
            pos.x -= Time.deltaTime * 4f;
            m_weapon.localPosition = pos;
            yield return null;
        }
        pos.x = 0;
        m_weapon.localPosition = pos;


        //再改变相机视距
        float fov = cameras[0].fieldOfView;
        while (fov > 45f)
        {
            fov -= Time.deltaTime * 300;
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].fieldOfView = fov;
            }
            yield return null;
        }
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].fieldOfView = 30f;
        }
    }

    IEnumerator DoCancAim()
    {

        //取消瞄准
        //先回归相机视距
        float fov = cameras[0].fieldOfView;
        while (fov < 45f)
        {
            fov += Time.deltaTime * 350;
            for (int i = 0; i < cameras.Length; i++)
            {
                cameras[i].fieldOfView = fov;
            }
            yield return null;
        }
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].fieldOfView = 60f;
        }

        //再回归武器位置
        Vector3 pos = m_weapon.localPosition;
        while (pos.x < weaponInitPos.x)
        {
            pos.x += Time.deltaTime * 7f;
            m_weapon.localPosition = pos;
            yield return null;
        }
        pos.x = weaponInitPos.x;
        m_weapon.localPosition = pos;

    }
    #endregion


    /// <summary>
    /// 玩家被攻击
    /// </summary>
    public void BeAttack(float attack = 1)
    {
        if (this.hp <= 0) return;
        this.hp -= (int)attack;
        if (hp <= 0)
        {
            //意思我
            hp = 0;
        }
        UIController.Instance.SetHP(this.hp);
    }

    /// <summary>
    /// 游戏控制
    /// </summary>
    void OnEventByGameController()
    {
        if (GameManager.Instance.GamePause)
        {
            //游戏暂停
            if (firstPersonController != null) firstPersonController.enabled = false;
        }
        else
        {
            //游戏继续
            if (firstPersonController != null) firstPersonController.enabled = true;
        }
    }


}
