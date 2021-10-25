using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class SelectDifficulty : UIEvent
{
    public Image bg;
    public SelectDifficulty[] otherDifficulties;
    bool isSelected = false;

    private void Start()
    {
        for (int i = 0; i < otherDifficulties.Length; i++)
        {
            otherDifficulties[i].myDelegate_OnClick -= OnSelected;
            otherDifficulties[i].myDelegate_OnClick += OnSelected;
        }

        if (GameConfig.Instance.currDifficulty.ToString().Equals(gameObject.name))
        {
            isSelected = true;
            transform.localScale = new Vector3(1.5f, 1f, 1f);
            bg.color = Color.green;
        }
        else
        {
            isSelected = false;
        }
    }

    void OnSelected(object obj)
    {
        isSelected = false;
        transform.localScale = new Vector3(1f, 1f, 1f);
        bg.color = Color.black;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        //Debug.LogError("鼠标点击");
        base.OnPointerClick(eventData);
        isSelected = true;
        GameConfig.Instance.currDifficulty = int.Parse(gameObject.name);
        GameConfig.Instance.RefleshDifficulty();
    }


    public override void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.LogError("鼠标悬浮");
        if (isSelected) return;
        transform.localScale = new Vector3(1.5f, 1f, 1f);
        bg.color = Color.black;
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        //Debug.LogError("鼠标退出");
        if (isSelected) return;
        transform.localScale = new Vector3(1f, 1f, 1f);
        bg.color = Color.black;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //Debug.LogError("鼠标按下");
        transform.localScale = new Vector3(1.5f, 1f, 1f);
        bg.color = Color.green;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //Debug.LogError("鼠标松开");
        transform.localScale = new Vector3(1.5f, 1f, 1f);
        bg.color = Color.green;
    }
}
