using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartControll : MonoBehaviour
{
    [Header("背景")]
    public Image bg;
    [Header("所有关卡")]
    public Button[] levelsBtns;
    [Header("无限模式")]
    public Button infiniteBtn;


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
        for (int i = 0; i < levelsBtns.Length; i++)
        {
            Button bt = levelsBtns[i];
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

        //无限模式
        if (infiniteBtn != null)
        {
            bool infinite = Sceneloading.Instance.IsInfinite;
            Button bt = levelsBtns[levelsBtns.Length - 1];

            infiniteBtn.interactable = bt.interactable;
            infiniteBtn.image.color = infinite ? Color.green : Color.red;
            infiniteBtn.transform.GetComponentInChildren<Text>().text = infinite ? "无限模式\n[开]" : "无限模式\n[关]";
            infiniteBtn.onClick.AddListener(delegate () { EnterLevel(!Sceneloading.Instance.IsInfinite); });
        }
    }


    /// <summary>
    /// 进入场景、关卡
    /// </summary>
    /// <param name="level">第几关卡</param>
    public void EnterLevel(int level)
    {
        bg.raycastTarget = true;
        Debug.Log("进入关卡【" + level + "】");
        Sceneloading.Instance.currLevel = level;
        Sceneloading.Instance.LoadScene(1);
    }

    /// <summary>
    /// 进入场景、关卡
    /// </summary>
    /// <param name="IsInfinite">true 开启无限模式</param>
    public void EnterLevel(bool IsInfinite)
    {
        Color c = IsInfinite ? Color.green : Color.red;
        infiniteBtn.image.color = c;
        infiniteBtn.transform.GetComponentInChildren<Text>().text = IsInfinite ? "无限模式\n[开]" : "无限模式\n[关]";
        Sceneloading.Instance.IsInfinite = IsInfinite;
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Sceneloading.Instance.AskByQuit(true);
        }
    }

}
