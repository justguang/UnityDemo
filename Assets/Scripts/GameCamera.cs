using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机
/// </summary>
public class GameCamera : MonoBehaviour
{
    public static GameCamera Instance;

    //相机距离地面的距离
    protected float m_distance = 12f;
    //相机的角度
    protected Vector3 m_rot = new Vector3(-75, 180, 0);
    //相机的移速
    protected float m_moveSpeed = 60;
    //相机的移动值
    protected float m_vx = 0;
    protected float m_vy = 0;

    public Transform m_transform;//自身点
    protected Transform m_cameraPoint;//相机点

    private void Awake()
    {
        Instance = this;
        m_transform = transform;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_cameraPoint = CameraPoint.Instance.transform;
        Follow();
    }

    private void LateUpdate()
    {
        Follow();
    }

    /// <summary>
    /// 相机对其到焦点的位置和角度
    /// </summary>
    void Follow()
    {
        //设置旋转角度
        m_cameraPoint.eulerAngles = m_rot;
        //将相机移动到指定位置
        m_transform.position = m_cameraPoint.TransformPoint(new Vector3(0, 0, m_distance));
        //将相机镜头对准目标点
        transform.LookAt(m_cameraPoint);

    }

    /// <summary>
    /// 控制相机移动
    /// </summary>
    /// <param name="mouse"></param>
    /// <param name="mx"></param>
    /// <param name="my"></param>
    public void Control(bool mouse, float mx, float my)
    {
        if (!mouse) return;

        m_cameraPoint.eulerAngles = Vector3.zero;

        //平移相机目标点
        m_cameraPoint.Translate(-mx, 0, -my);
    }

}
