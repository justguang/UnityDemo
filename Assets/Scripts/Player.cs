using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主角
/// </summary>
[AddComponentMenu("MyScript/Player")]
public class Player : MonoBehaviour
{

    public float m_speed = 1;//飞行速度
    public int m_life = 4;//生命

    Transform m_transform;

    public Transform m_rocket;//子弹预制体
    public float m_rocketTime = 0;//控制发射子弹时间

    public AudioClip m_shootClip;//声音文件
    protected AudioSource m_audio;//声音源
    public Transform m_explosionFX;//爆炸特效


    protected Vector3 m_targetPos;//目标位置
    public LayerMask m_inputMask;//鼠标射线碰撞层
    protected Camera m_camera;//相机


    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        m_audio = this.GetComponent<AudioSource>();
        m_targetPos = this.transform.position;//初始化目标点位置
        m_camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        m_rocketTime -= Time.deltaTime;
        if (m_rocketTime <= 0)
        {
            m_rocketTime = 0.13f;//每间隔0.15秒可以发射子弹

            //按空格键或鼠标左键发射子弹
            if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
            {
                //Instantiate(m_rocket, m_transform.position, m_transform.rotation);

                //利用对象池 进行子弹加载，避免内存多余的开销
                var p = PoolManager.Pools["mypool"];
                p.Spawn("PlayerRocket", m_transform.position, m_transform.rotation,null);


                //播放声音
                m_audio.PlayOneShot(m_shootClip);
            }
        }

        //判断运行平台，执行移动操作
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:

                //纵向移动距离
                float movev = 0;
                //横向移动距离
                float moveh = 0;

                //向上移动【z轴++】
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    movev += m_speed * Time.deltaTime;
                }

                //向下移动【z轴--】
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    movev -= m_speed * Time.deltaTime;
                }

                //向左移动【x轴--】
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    moveh -= m_speed * Time.deltaTime;
                }

                //向右移动【x轴++】
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    moveh += m_speed * Time.deltaTime;
                }

                //this.transform.Translate(new Vector3(moveh, 0, movev));//这种方式底层会不断执行getComponent<Transform>(),非常消耗性能不建议这样用，建议定义一个变量存储this.transform
                m_transform.Translate(new Vector3(moveh, 0, movev));//Translate函数其实执行的是向量加，等同于 transform.position += new Vector3(moveh,0,movev);
                break;

            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                MoveTo();
                break;
        }

    }

    void MoveTo()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 ms = Input.mousePosition;//获取鼠标屏幕位置
            Ray ray = m_camera.ScreenPointToRay(ms);//以屏幕位置转为射线
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, m_inputMask))
            {
                //如果射中目标，记录射线碰撞点的位置
                m_targetPos = hit.point;
            }
        }

        //使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
        Vector3 pos = Vector3.MoveTowards(m_transform.position, m_targetPos, m_speed * Time.deltaTime);

        //跟新当前位置
        m_transform.position = pos;
    }


    /// <summary>
    /// unity func：在碰撞体互相接触时触发此方法
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "PlayerRocket")
        {
            //如果撞到自己子弹以外的物体，生命-1，如果生命<=0，自我销毁
            m_life -= 1;
            GameManager.Instance.ChangeLife(m_life);//生命值改变
            if (m_life <= 0)
            {
                GameManager.Instance.ChangeLife(0);//生命值0
                //播放爆炸特效
                //Transform fx = Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
                Transform fx = PoolManager.Pools["mypool"].Spawn("Explosion", m_transform.position, Quaternion.identity);

                //GameManager.Instance.DelayDestoryFx(2, fx.gameObject);//2秒后销毁爆炸特效
                PoolManager.Pools["mypool"].Despawn(fx, 2);

                Destroy(this.gameObject);
            }
        }
    }
}
