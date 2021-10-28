using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TileWnd : EditorWindow
{
    //tile 脚本
    protected static TileObject tileObject;

    //添加菜单栏选项
    [MenuItem("myTools/Tile Window")]
    static void Create()
    {
        EditorWindow.GetWindow(typeof(TileWnd));

        //在场景中选中TileObject脚本实例
        if (Selection.activeTransform != null)
            tileObject = Selection.activeTransform.GetComponent<TileObject>();
    }

    //当改变选中的物体时更新
    private void OnSelectionChange()
    {
        if (Selection.activeTransform != null)
            tileObject = Selection.activeTransform.GetComponent<TileObject>();
    }

    //显示窗口UI
    private void OnGUI()
    {
        if (tileObject == null) return;

        //显示编辑器名称
        GUILayout.Label("Tile Editor");
        //在工程目录读取一张贴图
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GUI/butPlayer1.png");
        //将贴图显示在窗口内
        GUILayout.Label(tex);
        //是否显示帮助信息
        tileObject.debug = EditorGUILayout.Toggle("Debug", tileObject.debug);
        //切换 TileObject 的数据
        string[] editDataStr = { "禁止", "敌人通道", "守卫区" };
        tileObject.dataID = GUILayout.Toolbar(tileObject.dataID, editDataStr);

        EditorGUILayout.Separator();//分隔符
        if (GUILayout.Button("Reset"))
        {
            //重置
            tileObject.Init();//初始化
        }
    }

}
