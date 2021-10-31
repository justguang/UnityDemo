using UnityEngine;

/// <summary>
/// 相机点
/// </summary>
public class CameraPoint : MonoBehaviour
{
    public static CameraPoint Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// 在编辑器中显示一个图标
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "CameraPoint.tif");
    }
}
