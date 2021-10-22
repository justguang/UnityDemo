using PathologicalGames;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 僵尸的状态
/// </summary>
public enum ZombieState
{
    Idle,
    Walk,
    Run,
    Hurt,
    Attack,
    Death
}


/// <summary>
/// 僵尸控制
/// </summary>
public class ZombieController : MonoBehaviour
{
    #region 相关组件
    private Transform m_transform;//自身transform
    private NavMeshAgent m_navMeshAgent;//Ai导航
    private AudioSource m_audioSource;//音源
    private Animator m_ani;//动画控制
    private BoxCollider m_attackCollider;//攻击碰撞
    #endregion

    #region 自身数据
    private int m_hp;//血量
    private float m_walkSpeed;//行走速度
    private float m_runSpeed;//奔跑速度
    private float m_enmityTime;//僵尸仇恨值，如果被攻击，仇恨更高【玩家脱离僵尸检测范围，仇恨值逐渐减低，当等于0时，不再追逐玩家】
    private bool m_playerInCheck;//true玩家在检测范围内
    private Vector3 m_currNavigatePos = Vector3.zero;//当前导航目标点
    private Transform m_playerPoint;//玩家位置
    #endregion

    #region 音效
    [Header("音效")]
    public AudioClip[] m_moveAudioClips;//移动的音效
    public AudioClip[] m_idleAudioClips;//待机的音效
    public AudioClip[] m_hurtAudioClips;//受伤的音效
    public AudioClip[] m_attackAudioClips;//攻击的音效
    #endregion

    #region 僵尸状态
    [SerializeField] private ZombieState m_zombieState;

    public ZombieState ZombieState
    {
        get => m_zombieState;

        set
        {

            switch (value)
            {
                case ZombieState.Idle:
                    if (m_hp <= 0) return;
                    m_navMeshAgent.ResetPath();//清空导航路线
                    float delayTime = Random.Range(0f, 5f);
                    Invoke("NavigateNextPoint", delayTime);//延时几秒搜寻下一个目标点
                    m_ani.Play("Idle", 0, 0.0f);//播放动画
                    break;
                case ZombieState.Walk:
                    m_ani.Play("Walk", 0, 0.0f);
                    m_navMeshAgent.SetDestination(m_currNavigatePos);//设置AI导航点
                    m_navMeshAgent.speed = m_walkSpeed;
                    break;
                case ZombieState.Run:
                    if (m_hp <= 0) return;
                    m_ani.Play("Run", 0, 0.0f);
                    m_navMeshAgent.SetDestination(m_playerPoint.position);//追踪玩家
                    m_navMeshAgent.speed = m_runSpeed;
                    break;
                case ZombieState.Hurt:
                    if (m_hp <= 0) return;
                    m_navMeshAgent.ResetPath();
                    m_ani.Play("Hurt", 0, 0.0f);
                    break;
                case ZombieState.Attack:
                    if (m_hp <= 0) return;
                    m_navMeshAgent.ResetPath();
                    m_ani.Play("Attack", 0, 0.0f);
                    m_transform.LookAt(m_playerPoint);//面向玩家攻击
                    break;
                case ZombieState.Death:
                    m_navMeshAgent.ResetPath();
                    m_ani.Play("Death", 0, 0.0f);
                    m_transform.LookAt(m_playerPoint);//面向玩家后仰死亡

                    if (PoolManager.Pools["GamePool"].IsSpawned(m_transform))
                    {
                        PoolManager.Pools["GamePool"].Despawn(m_transform, 2.5f);//回收
                    }
                    else
                    {
                        Destroy(gameObject, 2.5f);
                    }

                    GameManager.Instance.DestroyOneForCurrZombieNum();

                    //Debug.LogError($"{gameObject.name}死亡");
                    break;
            }
            m_zombieState = value;
        }
    }
    #endregion

    void Start()
    {
        //Init();
        //订阅事件
        GameManager.Instance.GameControllerCallBack += OnEventByGameController;
    }


    /// <summary>
    /// 出生化
    /// </summary>
    public void Init()
    {
        #region 相关组件获取
        m_transform = this.transform;
        m_navMeshAgent = GetComponent<NavMeshAgent>();//Ai
        m_ani = GetComponent<Animator>();//动画控制
        m_audioSource = GetComponent<AudioSource>();//音源
        m_attackCollider = transform.Find("Hips/Spine/Spine1/Spine2/RightShoulder/RightArm/RightForeArm/RightHand/AttackCollider").GetComponent<BoxCollider>();//攻击的碰撞
        #endregion

        m_hp = 50;
        m_currNavigatePos = Vector3.zero;
        m_playerPoint = null;//删除玩家持有
        m_walkSpeed = 0.4f;//行走速度
        m_runSpeed = 4.5f;//奔跑速度

        m_playerInCheck = false;//玩家不在检测范围内
        m_enmityTime = 9f;//普通仇恨值（追逐时长）


        if (m_attackCollider != null) m_attackCollider.enabled = false;//取消激活攻击碰撞

        if (gameObject.activeSelf)
            ZombieState = ZombieState.Idle;
    }


    void Update()
    {
        if (GameManager.Instance.GamePause) return;
        if (m_hp <= 0)
        {
            if (ZombieState != ZombieState.Death)
            {
                ZombieState = ZombieState.Death;
            }

            return;
        }
        StateForUpdate();
        EnmityUpdate();
    }



    void StateForUpdate()
    {
        if (m_ani == null || m_ani.IsInTransition(0))
        {
            //跳过过渡期的动画
            return;
        }


        switch (m_zombieState)
        {
            case ZombieState.Idle:
                break;
            case ZombieState.Walk:
                //判断当前动画是 Walk动画，并且已播放完毕
                if (m_ani.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Walk")
                    && m_ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (Vector3.Distance(m_transform.position, m_currNavigatePos) <= 1.5f)
                    {
                        //接近AI导航目标点，进入Idle
                        ZombieState = ZombieState.Idle;
                    }
                }
                break;
            case ZombieState.Run:

                if (m_ani.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Run")
                && m_ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (m_playerPoint == null || !m_playerPoint.gameObject.activeSelf)
                    {
                        ZombieState = ZombieState.Idle;
                        return;
                    }
                    if (Vector3.Distance(m_transform.position, m_playerPoint.position) <= 1.5f)
                    {
                        //接近玩家，切换状态攻击
                        ZombieState = ZombieState.Attack;
                    }
                    else
                    {
                        m_navMeshAgent.SetDestination(m_playerPoint.position);
                    }
                }
                break;
            case ZombieState.Hurt:

                if (m_ani.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Hurt")
                    && m_ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (m_playerPoint == null || !m_playerPoint.gameObject.activeSelf)
                    {
                        ZombieState = ZombieState.Idle;
                        return;
                    }

                    //距离玩家近 = 攻击，反之跑向玩家
                    if (Vector3.Distance(m_transform.position, m_playerPoint.position) <= 1.5f)
                    {
                        ZombieState = ZombieState.Attack;
                    }
                    else
                    {
                        ZombieState = ZombieState.Run;
                    }
                }

                break;
            case ZombieState.Attack:

                if (m_ani.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("Attack")
                    && m_ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (m_playerPoint == null || !m_playerPoint.gameObject.activeSelf)
                    {
                        ZombieState = ZombieState.Idle;
                        return;
                    }

                    //距离玩家近 = 攻击，反之跑向玩家
                    if (Vector3.Distance(m_transform.position, m_playerPoint.position) <= 1.5f)
                    {
                        ZombieState = ZombieState.Attack;
                    }
                    else
                    {
                        ZombieState = ZombieState.Run;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 导航下一个目标点
    /// </summary>
    void NavigateNextPoint()
    {
        m_currNavigatePos = GameManager.Instance.GetOnePointByRandom(m_currNavigatePos);//获取新的导航目标点
        ZombieState = ZombieState.Walk;//切换走状态
    }

    /// <summary>
    /// 仇恨值更新
    /// </summary>
    void EnmityUpdate()
    {
        //要求存活、有玩家引用，并且玩家已脱离检测
        if (m_playerPoint == null) return;
        if (m_playerInCheck) return;
        m_enmityTime -= Time.deltaTime;
        if (m_enmityTime <= 0)
        {
            m_playerPoint = null;
            m_enmityTime = 9f;//重置仇恨值
            //Debug.LogError($"普通仇恨消除");
        }
    }

    /// <summary>
    /// 被攻击
    /// </summary>
    public void BeAttack(int attack = 1)
    {
        if (m_hp <= 0) return;
        m_hp -= attack;
        if (m_hp < 0)
        {
            //死亡
            m_hp = 0;
        }
        else
        {
            ZombieState = ZombieState.Hurt;//切换状态受伤
            if (m_playerPoint == null) m_playerPoint = PlayerController.Instance.transform;//一被攻击，僵尸就察觉到玩家的位置
            m_enmityTime = 18f;//仇恨值拉满
        }
    }


    #region 动画事件 -> 播放音效
    public void IdleAudio()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_idleAudioClips[Random.Range(0, m_idleAudioClips.Length)]);//随机播放Idle音效
    }
    public void MoveAudio()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_moveAudioClips[Random.Range(0, m_moveAudioClips.Length)]);//随机播放移动声音
    }
    public void HurtAudio()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_hurtAudioClips[Random.Range(0, m_hurtAudioClips.Length)]);//随机播放受伤惨叫
    }
    public void AttackAudio()
    {
        m_audioSource.Stop();
        m_audioSource.PlayOneShot(m_attackAudioClips[Random.Range(0, m_attackAudioClips.Length)]);//随机播放攻击音效
    }
    public void StartAttack()
    {
        if (m_attackCollider != null) m_attackCollider.enabled = true;
    }
    public void EndAttack()
    {
        if (m_attackCollider != null) m_attackCollider.enabled = false;
    }
    #endregion


    #region 事件相关
    /// <summary>
    /// 检测进入
    /// </summary>
    /// <param name="name">谁的检测</param>
    /// <param name="trans">检测到的目标</param>
    public void CheckEnter(string name, Transform trans)
    {
        switch (name)
        {
            case "Head":
                //Debug.LogError("检测到玩家");
                m_playerPoint = trans;
                m_playerInCheck = true;
                m_enmityTime = 9f;//刷新仇恨值
                ZombieState = ZombieState.Run;//切换状态
                break;
            case "AttackCollider":
                //Debug.LogError("攻击到玩家");
                PlayerController.Instance.BeAttack(2);
                break;
        }
    }

    /// <summary>
    /// 检测脱离
    /// </summary>
    /// <param name="name">谁的检测</param>
    /// <param name="trans">检测到的目标</param>
    public void CheckLeave(string name, Transform trans)
    {
        switch (name)
        {
            case "Head":
                //Debug.LogError("检测到玩家离开");
                m_playerInCheck = false;
                break;
            case "AttackCollider":
                //Debug.LogError("攻击脱离玩家");
                break;
        }
    }

    #endregion

    /// <summary>
    /// 游戏控制
    /// </summary>
    void OnEventByGameController()
    {
        if (GameManager.Instance.GamePause)
        {
            //游戏暂停

        }
        else
        {
            //游戏继续
            Init();
        }
    }

}
