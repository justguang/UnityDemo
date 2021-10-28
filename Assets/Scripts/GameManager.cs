using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Xml;
using System.IO;
using PathologicalGames;
using System.Linq;
using System.Text;

/// <summary>
/// 防守单位种类的配置
/// </summary>
[System.Serializable]
public struct DefenderConfig
{
    /// <summary>
    /// 防守单位的名字
    /// </summary>
    public string name;
    /// <summary>
    /// 攻击力
    /// </summary>
    public float power;
    /// <summary>
    /// 攻击范围
    /// </summary>
    public float attackArea;
    /// <summary>
    /// 攻击时间间隔
    /// </summary>
    public float attackInterval;
    /// <summary>
    /// 生成此防守单位需要支持的金币
    /// </summary>
    public int price;
}

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("相机")]
    public Camera m_camera;

    public LayerMask m_groundlayer;//地面的碰撞layer
    public int m_crrWave = 1;//波数
    public int m_life = 10;//生命值
    public int m_point = 30;//铜钱数量

    public bool m_debug = true;//显示路点的debug开关
    public List<PathNode> m_PathNodes;//所有路点
    public List<Enemy> m_enemyList = new List<Enemy>();//战斗所有生成的敌人

    [Header("敌人进攻波数配置")]
    public List<WaveData> waves;

    [Header("防守单位种类配置")]
    public List<DefenderConfig> defenderConfigs;

    //UI
    Text m_waveTxt;
    Text m_pointTxt;
    Text m_lifeTxt;
    Text m_tipsTxt;
    Vector3 m_tipsInitPos;
    Button m_onceAginBtn;//再试一次按钮

    //当前是否选中的创建防守单位的按钮
    bool m_isSelectedButton = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnApplicationQuit()
    {
        Quit();
    }

    private void OnDestroy()
    {
        Quit();
    }

    void Quit()
    {
        PoolManager.Pools["GamePool"].DespawnAll();
        m_PathNodes.Clear();
        m_enemyList.Clear();
        Instance = null;
        System.GC.Collect();
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 45;//设置帧率

        LoadConfig();

        //创建UnityActino，在OnButCreateDefenderDown函数中响应按钮按下事件
        UnityAction<BaseEventData> downAction = new UnityAction<BaseEventData>(OnButCreateDefenderDown);
        //创建UnityActino，在OnButCreateDefenderUp函数中响应按钮抬起事件
        UnityAction<BaseEventData> upAction = new UnityAction<BaseEventData>(OnButCreateDefenderUp);

        //按钮按下事件
        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(downAction);

        //按钮抬起事件
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(upAction);

        //查找所有子物体，根据名称获取UI控件
        Transform[] childs = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            Transform t = childs[i];
            if (t.name.Equals("wave"))
            {
                //设置波数
                m_waveTxt = t.GetComponent<Text>();
                SetWave(0);
            }
            else if (t.name.Equals("life"))
            {
                //显示生命
                m_lifeTxt = t.GetComponent<Text>();
                m_lifeTxt.text = string.Format("生命：{0}", m_life);
            }
            else if (t.name.Equals("point"))
            {
                //设置铜钱
                m_pointTxt = t.GetComponent<Text>();
                m_pointTxt.text = string.Format("铜钱：{0}", m_point);
            }
            else if (t.name.Equals("tips"))
            {
                //tips消息提示
                m_tipsTxt = t.GetComponent<Text>();
                m_tipsInitPos = m_tipsTxt.transform.localPosition;
                m_tipsTxt.gameObject.SetActive(false);
            }
            else if (t.name.Equals("btn_onceAgin"))
            {
                //再试一次按钮
                m_onceAginBtn = t.GetComponent<Button>();
                m_onceAginBtn.onClick.AddListener(delegate ()
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });

                //默认隐藏按钮
                m_onceAginBtn.gameObject.SetActive(false);
            }
            else if (t.name.Contains("btn_player"))
            {

                //防守单位按钮
                EventTrigger trigger = t.gameObject.AddComponent<EventTrigger>();
                trigger.triggers = new List<EventTrigger.Entry>();
                trigger.triggers.Add(down);
                trigger.triggers.Add(up);
            }
        }


        if (m_PathNodes == null || m_PathNodes.Count <= 0) BuildPath();


    }

    #region 配置文件相关
    /// <summary>
    /// 加载配置
    /// </summary>
    public void LoadConfig()
    {
        waves = new List<WaveData>();
        defenderConfigs = new List<DefenderConfig>();
        m_life = 10;
        m_point = 30;

        /*
                Debug.LogError("游戏数据存储路径streamingAssetsPath= " + Application.streamingAssetsPath);
                Debug.LogError("游戏数据存储路径persistentDataPath= " + Application.persistentDataPath);
                Debug.LogError("游戏运行日志存储路径 " + Application.consoleLogPath);

                GameConfigEdit.xml 和 GameConfig.xml 内容一样
                GameConfigEdit.xml 方便编写、可读性好
                GameConfig.xml 是压缩后的内容，占用空间小
        */



        string filePath = Application.persistentDataPath + "/GameConfigEdit.xml";
        if (!File.Exists(filePath))
        {
            Debug.LogError("游戏配置文件不存在,生成中... ：" + filePath);
            CreateGameConfigFile();
        }

        XmlDocument xmlDoc = new XmlDocument();
        //忽略xml文件的注释
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;
        XmlReader xr = XmlReader.Create(filePath, settings);
        xmlDoc.Load(xr);

        XmlNode GameConfigNode = xmlDoc.SelectSingleNode("GameConfig");
        XmlNodeList nodes = GameConfigNode.ChildNodes;

        for (int i = 0; i < nodes.Count; i++)
        {
            XmlNode n = nodes[i];
            switch (n.Name)
            {
                //配置敌人进攻波数
                case "wave":
                    waves.Add(new WaveData
                    {
                        waveID = int.Parse(n.Attributes["id"].Value),
                        enemyArr = n.Attributes["enemy"].Value.Split('|'),
                        level = int.Parse(n.Attributes["enemyLv"].Value),
                        baseHp = float.Parse(n.Attributes["baseHp"].Value),
                        baseSpeed = float.Parse(n.Attributes["baseSpeed"].Value),
                        price = int.Parse(n.Attributes["price"].Value),
                        interval = float.Parse(n.Attributes["generateInterval"].Value)
                    });
                    break;

                //配置防守单位的种类
                case "defender":
                    defenderConfigs.Add(new DefenderConfig
                    {
                        name = n.Attributes["name"].Value,
                        power = float.Parse(n.Attributes["power"].Value),
                        attackArea = float.Parse(n.Attributes["attackArea"].Value),
                        attackInterval = float.Parse(n.Attributes["attackInterval"].Value),
                        price = int.Parse(n.Attributes["price"].Value)
                    });

                    break;
            }
        }

        //按waveID升序排序，保证波数顺序
        if (waves.Count > 0)
        {
            waves.Sort((w1, w2) =>
            {
                return w1.waveID.CompareTo(w2.waveID);
            });
        }

    }

    /// <summary>
    /// 生成配置文件
    /// </summary>
    /// <returns></returns>
    void CreateGameConfigFile()
    {
        string toPath = Application.persistentDataPath + "/GameConfigEdit.xml";
        TextAsset textAsset = (TextAsset)Resources.Load("GameConfig/GameConfigEdit", typeof(TextAsset));

        if (File.Exists(toPath))
            File.Delete(toPath);

        File.WriteAllText(toPath, textAsset.text, Encoding.UTF8);
    }

    #endregion

    /// <summary>
    /// 设置波数
    /// </summary>
    /// <param name="wave"></param>
    public void SetWave(int wave)
    {
        m_crrWave = wave;
        m_waveTxt.text = string.Format("波数：{0}/{1}", m_crrWave, waves.Count);
    }


    /// <summary>
    /// 设置生命值
    /// </summary>
    /// <param name="damage">受到伤害</param>
    public void SetDamage(int damage)
    {
        m_life -= damage;
        if (m_life <= 0)
        {
            //死亡
            m_life = 0;
            ShowMsg("通关失败！！", Color.red);
            RestartGame();
        }
        m_lifeTxt.text = string.Format("生命：{0}", m_life);
    }

    /// <summary>
    /// 铜钱是否足够
    /// </summary>
    /// <param name="defenderName"></param>
    /// <returns></returns>
    public bool PointEnough(string defenderName)
    {
        bool isEnough = false;
        if (defenderName.Contains("1"))
        {
            isEnough = m_point >= defenderConfigs.Where(d => d.name.Equals("SwordMan")).First().price;

        }
        else if (defenderName.Contains("2"))
        {
            isEnough = m_point >= defenderConfigs.Where(d => d.name.Equals("Archer")).First().price;
        }
        if (!isEnough)
        {
            ShowMsg("铜钱不足", Color.yellow);
        }
        return isEnough;
    }

    /// <summary>
    /// 设置铜钱
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool SetPoint(int point)
    {
        if ((m_point + point) < 0)
        {
            //铜钱不够
            return false;
        }

        m_point += point;
        m_pointTxt.text = string.Format("铜钱：{0}", m_point);
        return true;
    }

    /// <summary>
    /// 显示消息
    /// </summary>
    /// <param name="msg">内容</param>
    /// <param name="textColor">内容文本颜色</param>
    public void ShowMsg(string msg, Color textColor)
    {
        StopCoroutine("DoTips");
        StartCoroutine("DoTips", new object[] { msg, textColor });
    }

    /// <summary>
    /// tips内容弹窗
    /// </summary>
    /// <param name="msg">内容</param>
    /// <returns></returns>
    IEnumerator DoTips(params object[] msg)
    {
        m_tipsTxt.text = msg[0].ToString();
        m_tipsTxt.color = (Color)msg[1];
        m_tipsTxt.transform.localPosition = m_tipsInitPos;
        Vector3 movePos = m_tipsTxt.transform.localPosition;
        m_tipsTxt.gameObject.SetActive(true);
        while (movePos.y <= 150)
        {
            movePos.y += Time.deltaTime * 70f;
            m_tipsTxt.transform.localPosition = movePos;
            yield return 0;
        }
        m_tipsTxt.gameObject.SetActive(false);
        m_tipsTxt.transform.localPosition = m_tipsInitPos;
        yield break;
    }


    /// <summary>
    /// 按下 “创建防守单位”按钮
    /// </summary>
    /// <param name="data"></param>
    void OnButCreateDefenderDown(BaseEventData data)
    {
        if (PointEnough(data.selectedObject.name))
        {
            //铜钱充足
            data.selectedObject.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f);

            m_isSelectedButton = true;
        }


    }


    /// <summary>
    /// 抬起“创建防守单位”按钮
    /// </summary>
    /// <param name="data"></param>
    void OnButCreateDefenderUp(BaseEventData data)
    {
        if (!m_isSelectedButton) return;//铜钱不足
        data.selectedObject.GetComponent<RectTransform>().localScale = Vector3.one;

        //创建射线
        Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
        //检测是否与地面碰撞
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, m_groundlayer))
        {
            //如果选中的是一个可用的格子
            if (TileObject.Instance.GetDataFromPosition(hit.point.x, hit.point.z) == (int)TileSatus.GUARD)
            {
                //获取碰撞点
                Vector3 hitPos = new Vector3(hit.point.x, 0, hit.point.z);
                //获取Grid Object座位位置
                Vector3 gridPos = TileObject.Instance.m_transform.position;
                //获取格子大小
                float tileSize = TileObject.Instance.tileSize;
                //计算出鼠标点的格子的中心位置
                hitPos.x = gridPos.x + (int)((hitPos.x - gridPos.x) / tileSize) * tileSize + tileSize * 0.5f;
                hitPos.z = gridPos.z + (int)((hitPos.z - gridPos.z) / tileSize) * tileSize + tileSize * 0.5f;

                //获得选择的按钮GameObject，将简单通过按钮名字判断选择了哪个按钮(防守单位)
                GameObject selectedGo = data.selectedObject;
                if (selectedGo.name.Contains("1"))
                {
                    DefenderConfig dcf = defenderConfigs.Where(d => d.name.Equals("SwordMan")).First();
                    //扣除响应铜钱，如果够的话
                    if (SetPoint(-dcf.price))
                    {
                        //创建近战防守单位
                        Defender.Create<Defender>(dcf, hitPos, new Vector3(0, 180, 0));
                    }
                }
                else if (selectedGo.name.Contains("2"))
                {
                    DefenderConfig dcf = defenderConfigs.Where(d => d.name.Equals("Archer")).First();
                    if (SetPoint(-dcf.price))
                    {
                        //创建远程防守单位
                        Defender.Create<ArcherDefender>(dcf, hitPos, new Vector3(0, 180, 0));
                    }
                }
            }

        }

        m_isSelectedButton = false;
    }

    public void RestartGame()
    {
        m_onceAginBtn.gameObject.SetActive(true);//显示再试一次（重新游戏）按钮
    }



    // Update is called once per frame
    void Update()
    {

        //如果选中创建士兵的按钮，则取消相机操作
        if (m_isSelectedButton)
        {
            return;
        }

        //不同平台的Input代码不同
#if(UNITY_IOS||UNITY_ANDROID)&&!UNITY_EDITOR
        bool press=Input.touches.Length>0?true:false;//获取手指是否触屏
        float mx=0;
        float my=0;
        if(press)
        {
            if(Input.GetTouch(0).phase==TouchPhase.Moved)//获得手指移动的距离
            {
                mx=Input.GetTouch(0).deltaPosition.x*0.01f;
                my=Input.GetTouch(0).deltaPosition.y*0.01f;
            }
        }
#else
        bool press = Input.GetMouseButton(0);
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
#endif
        

        //移动相机
        GameCamera.Instance.Control(press, mx, my);
    }



    [ContextMenu("BuildPath")]
    public void BuildPath()
    {
        m_PathNodes = new List<PathNode>();
        //通过路点的tag查找所有的路点
        GameObject[] objs = GameObject.FindGameObjectsWithTag("pathnode");

        for (int i = 0; i < objs.Length; i++)
        {
            m_PathNodes.Add(objs[i].GetComponent<PathNode>());
        }
    }



    private void OnDrawGizmos()
    {
        if (!m_debug || m_PathNodes == null) return;
        Gizmos.color = Color.cyan;//将路点连线的颜色射为青色

        //遍历所有路点
        foreach (PathNode node in m_PathNodes)
        {
            if (node.m_next != null)
            {
                //在路点之间画出连线
                Gizmos.DrawLine(node.transform.position, node.m_next.transform.position);
            }
        }
    }
}
