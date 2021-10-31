using UnityEngine;

/// <summary>
/// 路点信息
/// </summary>
public class PathNode : MonoBehaviour
{
    public Transform m_transform;//自身的transform;
    public PathNode m_parent;//前一个结点
    public PathNode m_next;//下一个结点

    private void Start()
    {
        m_transform = transform;
    }

    public void SetNext(PathNode pathNode)
    {
        if (m_next != null) m_next.m_parent = null;
        m_next = pathNode;
        pathNode.m_parent = this;
    }

    //在编辑器中显示图标
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Node.tif");
    }


}
