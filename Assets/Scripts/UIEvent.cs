using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;
/// <summary>
/// UI事件
/// </summary>
public class UIEvent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action<object> myDelegate_OnClick;
    //public Image BgLine;
    //public Image Icon;
    //public Text Title;


    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //Debug.LogError("鼠标点击");
        if (eventData.button.Equals(InputButton.Left))
        {
            myDelegate_OnClick(this.gameObject);
        }
    }


    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.LogError("鼠标悬浮");
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //Debug.LogError("鼠标退出");
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //Debug.LogError("鼠标按下");
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        //Debug.LogError("鼠标松开");
    }
}

