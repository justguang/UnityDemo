using UnityEngine;
using System.Collections;
using PathologicalGames;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
    public bool OnlyDeactivate;
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!ps.IsAlive(true))
            {
                if (OnlyDeactivate)
                {
#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
#else
                    this.gameObject.SetActive(false);
#endif
                }
                else
                {
                    //ªÿ ’
                    SpawnPool p = PoolManager.Pools["GamePool"];
                    if (p.IsSpawned(transform))
                        p.Despawn(transform);
                    else
                        GameObject.Destroy(this.gameObject);
                }
                break;
            }
        }
    }
}
