using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小地图
/// </summary>
[AddComponentMenu("Game/MiniCamera")]
public class MiniCamera : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        float ratio = (float)Screen.width / (float)Screen.height;//获得屏幕的分辨率比例
        //使相机试图永远是一个正方向，rect的前两个参数表示XY位置，后两个参数是XY大小
        this.GetComponent<Camera>().rect = new Rect((1 - 0.2f), (1 - 0.2f * ratio), 0.2f, 0.2f * ratio);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
