using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗记录历史最高分
/// </summary>
public class Record : MonoBehaviour
{
    private string recordPath = "/record.txt";
    public Text text;
    public int maxRecord;//历史最高分


    /// <summary>
    /// 刷新
    /// </summary>
    public void RefleshRecord(bool isActive, int record = 0)
    {
        string filePath = Application.streamingAssetsPath + recordPath;
        Debug.Log($"最高分记录文件位置【{filePath}】");

        //确认获取Text组件
        if (text == null) text = transform.Find("record")?.GetComponent<Text>();
        if (text == null) throw new Exception("没有最高分的UI显示组件(Text),无法显示历史最高分");

        this.gameObject.SetActive(isActive);

        if (record > maxRecord)
        {
            //将新分数写入文件
            File.WriteAllText(filePath, record.ToString(), Encoding.UTF8);
            Debug.Log($"历史最高分：{maxRecord}，刷新最高分：{record}");

            maxRecord = record;
            if (text != null) text.text = record.ToString();
        }
        else
        {
            if (maxRecord > 0) return;

            //重新读取文件内容，将分数显示在UI
            string[] readStr = File.ReadAllLines(filePath, Encoding.UTF8);
            if (readStr != null) int.TryParse(readStr[0], out maxRecord);


            if (text != null) text.text = maxRecord.ToString();
            Debug.Log($"读取历史最高分：{maxRecord}");
        }

    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void CloseRecord()
    {
        this.gameObject.SetActive(false);
    }


    /// <summary>
    /// 显示
    /// </summary>
    public void ShowRecord()
    {
        this.gameObject.SetActive(true);
    }
}
