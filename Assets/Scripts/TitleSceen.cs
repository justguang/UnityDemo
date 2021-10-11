using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("MyScript/TitleSceen")]
public class TitleSceen : MonoBehaviour
{
    public void OnButtonGameStart()
    {
        SceneManager.LoadScene("level1");//读取关卡level1
    }
}
