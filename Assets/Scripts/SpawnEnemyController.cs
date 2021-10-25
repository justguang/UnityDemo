using PathologicalGames;
using System.Collections;
using UnityEngine;

public class SpawnEnemyController : MonoBehaviour
{
    private float timer;
    // Start is called before the first frame update
    void Start()
    {

        Invoke("DelayGenerateZombie", Random.Range(2f, 5f));
    }

    void DelayGenerateZombie()
    {
        StartCoroutine(DoGenerateZombie());
    }

    IEnumerator DoGenerateZombie()
    {

        if (GameConfig.Instance.Infinity)
        {
            //无限模式
            while (true)
            {
                timer = Random.Range(2f, 10f);//定时生成僵尸
                yield return new WaitForSeconds(timer);
                if (GameManager.Instance.CurrZombieNum < GameManager.Instance.MaxZombieNum)
                {
                    Transform obj = PoolManager.Pools["GamePool"].Spawn("Zombie", transform.position, Quaternion.identity, transform);
                    obj.GetComponent<ZombieController>().Init();
                    GameManager.Instance.AddOneForCurrZombieNum();
                }

            }
        }
        else
        {
            int maxNum = GameManager.Instance.MaxZombieNum;
            while (GameManager.Instance.CurrZombieNum < maxNum)
            {
                timer = Random.Range(1f, 5f);//定时生成僵尸
                yield return new WaitForSeconds(timer);
                if (GameManager.Instance.CurrZombieNum < maxNum)
                {
                    Transform obj = PoolManager.Pools["GamePool"].Spawn("Zombie", transform.position, Quaternion.identity, transform);
                    obj.GetComponent<ZombieController>().Init();
                    GameManager.Instance.AddOneForCurrZombieNum();
                }

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "item.png");
    }
}
