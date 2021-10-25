using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class UIButton : UIEvent
{

    public Image Icon;
    public Text Title;
    public Image BgLine;

    private void Start()
    {
        Icon.color = new Color(1, 1, 1, 0);
        BgLine.color = new Color(1, 1, 1, 0);
        Title.color = new Color(0.7f, 0.7f, 0.7f, 1);
        Title.fontSize = 50;
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        //Debug.LogError("鼠标点击");
        base.OnPointerClick(eventData);
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.LogError("鼠标悬浮");
        Icon.color = Color.white;
        BgLine.color = Color.white;
        Title.fontSize = 70;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //Debug.LogError("鼠标退出");
        Icon.color = new Color(1, 1, 1, 0);
        BgLine.color = new Color(1, 1, 1, 0);
        Title.color = new Color(0.7f, 0.7f, 0.7f, 1);
        Title.fontSize = 50;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Title.color = Color.white;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        Title.color = new Color(0.7f, 0.7f, 0.7f, 1);
    }
}
