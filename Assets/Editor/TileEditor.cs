using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(TileObject))]
public class TileEditor : Editor
{
    //是否处于编辑模式p
    protected bool editModel = false;
    //受编辑器影响的tile脚本
    protected TileObject tileObject;

    private void OnEnable()
    {
        //获得tile脚本
        tileObject = (TileObject)target;
    }

    //更改场景中的操作
    public void OnSceneGUI()
    {
        //如果在编辑模式下
        if (editModel)
        {
            //取消编辑器的选择功能
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            //在编辑器中显示数据（画出辅助线）
            tileObject.debug = true;
            //获取 Input 事件
            Event e = Event.current;

            //如果时鼠标左键
            if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && !e.alt)
            {
                //获取由鼠标位置产生的射线
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                //计算碰撞
                if (Physics.Raycast(ray, out RaycastHit hit, 2000, tileObject.tileLayer))
                {
                    tileObject.SetDataFromPosition(hit.point.x, hit.point.z, tileObject.dataID);
                }
            }
        }

        HandleUtility.Repaint();
    }


    /// <summary>
    /// 自定义 Inspector 窗口的UI
    /// </summary>
    public override void OnInspectorGUI()
    {
        GUILayout.Label("Tile Editor");//显示编辑器名称
        editModel = EditorGUILayout.Toggle("Edit", editModel);//是否启动编辑模式
        tileObject.debug = EditorGUILayout.Toggle("Debug",tileObject.debug);//是否显示信息

        string[] editDataStr = { "禁止区","敌人通道","守卫区"};
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);

        EditorGUILayout.Separator();//分隔符
        if (GUILayout.Button("Reset"))
        {
            //重置
            tileObject.Init();//初始化
        }
        DrawDefaultInspector();
    }

}


