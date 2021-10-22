using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家死亡，进入漫游状态
/// </summary>
public class RoamController : MonoBehaviour
{
    bool isDisable;//true代表已经取消激活过

    private void OnDisable()
    {
        isDisable = true;
    }

    private void OnEnable()
    {
        if (!isDisable) return;
        isDisable = false;

        System.GC.Collect();
        Debug.Log("进入漫游");
        //隐藏所有子级物体【相机除外】
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(0).name.Equals("CameraByWeapon"))
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }


        //调整相机位置
        StartCoroutine(DoInitPos());
        StartCoroutine(DoInitRot());
    }


    IEnumerator DoInitPos()
    {
        Vector3 pos = transform.localPosition;
        while (pos.y < 20f)
        {
            pos.y += Time.deltaTime * 20f;
            transform.localPosition = pos;
            yield return null;
        }

        pos.y = 20f;
        transform.localPosition = pos;
    }


    IEnumerator DoInitRot()
    {
        Vector3 rot = transform.localEulerAngles;
        while (rot.x >= 270f && rot.x <= 361f)
        {
            rot.x += Time.deltaTime * 100f;
            transform.localEulerAngles = rot;
            yield return null;
        }

        rot = transform.localEulerAngles;
        while (rot.x <= 75f)
        {
            rot.x += Time.deltaTime * 60f;
            transform.localEulerAngles = rot;
            yield return null;
        }

        rot = transform.localEulerAngles;
        while (rot.x > 80f)
        {
            rot.x -= Time.deltaTime * 30f;
            transform.localEulerAngles = rot;
            yield return null;
        }

        rot.x = 80f;
        transform.localEulerAngles = rot;
    }

}
