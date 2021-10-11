using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("MyScript/TitleSceen")]
public class TitleSceen : MonoBehaviour
{
    public Record record;//历史最高分

    private void Start()
    {
        if (record == null)
        {
            record = GameObject.Find("Record")?.GetComponent<Record>();
        }
        if (record != null) record.RefleshRecord(false);
    }

    public void OnButtonGameStart()
    {
        SceneManager.LoadScene("level1");//读取关卡level1

    }


}
