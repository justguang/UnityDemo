using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartControll : MonoBehaviour
{
    //所有按钮、关卡
    public Button[] btns;
    public Image bg;

    void Start()
    {
        if (Sceneloading.Instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/SceneLoading");
            GameObject sceneLoading = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        StartCoroutine(DoUpdateLevel());
        bg.raycastTarget = false;
    }

    /// <summary>
    /// 跟新关卡
    /// </summary>
    /// <returns></returns>
    IEnumerator DoUpdateLevel()
    {
        yield return 5;


        //添加按钮功能
        for (int i = 0; i < btns.Length; i++)
        {
            Button bt = btns[i];
            int btnName = int.Parse(bt.name);
            //添加点击事件
            bt.onClick.AddListener(delegate () { EnterLevel(btnName); });

            //判读是否为解锁关卡
            if (btnName <= Sceneloading.Instance.lastLevel)
            {
                //未解锁的关卡
                bt.interactable = true;
                bt.image.color = Color.white;
            }
            else
            {
                //已解锁的关卡
                bt.interactable = false;
                bt.image.color = Color.gray;
            }

        }
    }


    /// <summary>
    /// 进入场景、关卡
    /// </summary>
    /// <param name="level"></param>
    public void EnterLevel(int level)
    {
        bg.raycastTarget = true;
        Debug.Log("进入关卡【" + level + "】");
        Sceneloading.Instance.currLevel = level;
        Sceneloading.Instance.LoadScene(1);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Sceneloading.Instance.AskByQuit(true);
        }
    }

}
