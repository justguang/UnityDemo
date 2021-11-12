using UnityEngine;

/// <summary>
/// 敌人=》鸟
/// </summary>
public class BirdEnemy : Enemy
{
    private void Update()
    {
        if (m_currentNode == null) return;
        RotateTo();
        MoveTo();
        Fly();
    }

    public void Fly()
    {
        float flySpeed = 0;
        if (m_transform.position.y < 2.0f)
        {
            flySpeed = 1.0f;
        }
        m_transform.Translate(new Vector3(0, flySpeed * Time.deltaTime, 0));
    }
}
