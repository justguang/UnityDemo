using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SpawnEnemyController : MonoBehaviour
{
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoGenerateZombie());
    }


    IEnumerator DoGenerateZombie()
    {
        while (true)
        {
            timer = Random.Range(2f, 10f);//定时生成僵尸
            yield return new WaitForSeconds(timer);
            if (GameManager.Instance.CanGenerateZombie)
            {
                Transform obj = PoolManager.Pools["GamePool"].Spawn("Zombie", transform.position, Quaternion.identity, transform);
                GameManager.Instance.AddOneForCurrZombieNum();
                obj.GetComponent<ZombieController>().Init();
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "item.png");
    }
}
