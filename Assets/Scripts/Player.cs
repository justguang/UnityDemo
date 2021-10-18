using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 玩家控制脚本
/// </summary>
[AddComponentMenu("Game/Player")]
public class Player : MonoBehaviour
{

    public Transform m_transform;//自身transform
    Transform m_camera;//相机
    Vector3 m_camRot;//相机角度
    float m_camHeight = 1.4f;//相机高度


    CharacterController m_ch;//角色控制组件
    float m_moveSpeed = 3.0f;//角色移速
    float m_gravity = 2.0f;//重力
    public int m_life = 5;//生命值

    Transform m_muzzlePoint;//枪口点
    public LayerMask m_attackLayer;//攻击时，子弹（射线）能碰撞的层
    public Transform m_fx;//击中目标后的粒子效果
    public AudioClip m_shootAudioClip;//射击音效
    AudioSource m_audio;//音源组件
    float m_shootTimer = 0;//射击时间间隔计时器



    // Start is called before the first frame update
    void Start()
    {
        m_transform = this.transform;
        m_ch = this.GetComponent<CharacterController>();//获取角色控制器组件

        m_camera = Camera.main.transform;//获取场景相机
        //设置相机初始位置（使用TransformPoint获取Player在Y轴偏移一定高度的位置）
        m_camera.position = m_transform.TransformPoint(0, m_camHeight, 0);

        //设置相机等待旋转方向与主角一致
        m_camera.rotation = m_transform.rotation;
        m_camRot = m_camera.eulerAngles;

        //获取枪口点
        m_muzzlePoint = m_camera.Find("M16/weapon/muzzlepoint");
        //获取音源组件
        m_audio = GetComponent<AudioSource>();

        //锁定鼠标
        //Screen.lockCursor = true;//该方法已过时用 Cursor.lockState、 Cursor.visible 代替
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_attackLayer = LayerMask.GetMask(new string[] { "enemy", "level" });
    }


    // Update is called once per frame
    void Update()
    {
        //如果生命值未0  什么也不做
        if (m_life <= 0) return;

        Control();

        //更新射击间隔时间
        m_shootTimer -= Time.deltaTime;
        //鼠标左键射击
        if (Input.GetMouseButton(0) && m_shootTimer <= 0)
        {
            m_shootTimer = 0.1f;//重置射击间隔时间
            m_audio.PlayOneShot(m_shootAudioClip);//播放射击音效
            GameManager.Instance.ReduceAmmo(1);//减少弹药

            //利用Physics提供的功能，从相机位置，向前方（z轴）发射一条100长的射线，指定碰撞（m_attackLayer）层级，如果碰撞到返回true，反之false，RaycastHit是碰撞到的目标信息
            if (Physics.Raycast(m_muzzlePoint.position, m_camera.TransformDirection(Vector3.forward), out RaycastHit info, 200, m_attackLayer))
            {
                //判断碰撞到的信息（info）
                if (info.transform.tag.Equals("enemy"))
                {
                    //如果碰撞到的是敌人
                    info.transform.GetComponent<Enemy>()?.OnDamage(1);//敌人生命减少1
                }

                //在射中的敌方释放一个粒子效果
                Instantiate(m_fx, info.point, info.transform.rotation);
            }

        }
    }

    /// <summary>
    /// 控制角色移动
    /// </summary>
    void Control()
    {
        //获取鼠标移动距离
        float rh = Input.GetAxis("Mouse X");
        float rv = Input.GetAxis("Mouse Y");

        //旋转相机
        m_camRot.x -= rv;
        m_camRot.y += rh;
        m_camera.eulerAngles = m_camRot;
        //使主角方向与相机一致
        Vector3 camrot = m_camera.eulerAngles;
        camrot.x = 0; camrot.z = 0;
        m_transform.eulerAngles = camrot;


        Vector3 motion = Vector3.zero;//移动方向
        motion.x = Input.GetAxis("Horizontal") * m_moveSpeed * Time.deltaTime;
        motion.z = Input.GetAxis("Vertical") * m_moveSpeed * Time.deltaTime;
        //重力
        motion.y -= m_gravity * Time.deltaTime;
        //使用角色控制器提供的Move方法进行移动，它会自动检测碰撞
        m_ch.Move(m_transform.TransformDirection(motion));


        //更新相机位置 让相机始终与主角一致
        m_camera.position = m_transform.TransformPoint(0, m_camHeight, 0);
    }

    /// <summary>
    /// 主角被攻击
    /// </summary>
    /// <param name="damage"></param>
    public void OnDamage(int damage)
    {
        m_life -= damage;
        //更新UI
        GameManager.Instance.SetLife(m_life);

        //如果生命值小于等于0，取消鼠标光标的锁定
        if (m_life <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }


    /// <summary>
    /// 显示主角的图标
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "Spawn.tif");
    }
}
