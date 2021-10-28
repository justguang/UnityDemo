using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public static TileObject Instance;
    public Transform m_transform;

    //tile碰撞层
    public LayerMask tileLayer;

    public float tileSize = 1;//大小
    public int xTileCount = 2;//x轴方向的tile数量
    public int zTileCount = 2;//z轴方向的tile数量
    public int[] data;//格子数值，0锁定，无法摆放任何物体；1敌人通道；2可以摆放防守单位
    public Transform[] mapsData;//生成的格子

    [HideInInspector]
    public int dataID = 0;

    [HideInInspector]
    public bool debug = false;//是否显示数据信息

    private void Awake()
    {
        Instance = this;
        m_transform = this.transform;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// 初始化地图数据
    /// </summary>
    public void Init()
    {
        data = new int[xTileCount * zTileCount];

        for (int i = 0; i < mapsData.Length; i++)
        {
            Transform t = mapsData[i];
            if (t != null)
            {
                mapsData[i] = null;
                GameObject.DestroyImmediate(t.gameObject);
            }
        }
        mapsData = new Transform[xTileCount * zTileCount];
    }

    /// <summary>
    /// 获得相应的tile数值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public int GetDataFromPosition(float x, float z)
    {
        int index = (int)((x - m_transform.position.x) / tileSize) * zTileCount + (int)((z - m_transform.position.z) / tileSize);
        if (index < 0 || index >= data.Length) return 0;

        return data[index];
    }

    public void SetDataFromPosition(float x, float z, int number)
    {
        int index = (int)((x - m_transform.position.x) / tileSize) * zTileCount + (int)((z - m_transform.position.z) / tileSize);
        if (index < 0 || index >= data.Length) return;
        data[index] = number;
    }


    /// <summary>
    /// 在编辑模式下显示相关信息
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!debug) return;

        if (data == null)
        {
            Debug.Log("无数据，请先Init初始化");
            return;
        }

        Vector3 pos = transform.position;
        //画z轴方向辅助线
        for (int i = 0; i < xTileCount; i++)
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(tileSize * i, pos.y, 0), transform.TransformPoint(tileSize * i, pos.y, tileSize * zTileCount));

            //高亮显示当前数值的格子
            for (int j = 0; j < zTileCount; j++)
            {
                if ((i * zTileCount + j) < data.Length && data[i * zTileCount + j] == dataID)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.3f);
                    Vector3 point = new Vector3(pos.x + i * tileSize + tileSize * 0.5f, pos.y, pos.z + j * tileSize + tileSize * 0.5f);
                    Gizmos.DrawCube(point, new Vector3(tileSize, 0.2f, tileSize));
                    string mapPrefabPath_prefix = "Prefabs/";
                    string mapPrefabPath_postfix = "";
                    if (dataID == (int)TileSatus.GUARD)
                    {
                        //防守单位区域
                        mapPrefabPath_postfix = "DefenderRoad";
                    }
                    else if (dataID == (int)TileSatus.ROAD)
                    {
                        //敌人通过的区域
                        mapPrefabPath_postfix = "EnemyRoad";
                    }

                    //获取对应下标下的mapsData，如果存在对象并且不是对应mapData，删除
                    Transform mapTrans = mapsData[i * zTileCount + j];
                    int onHierarchyIdx = -1;
                    if (mapTrans != null && !mapTrans.name.Contains(mapPrefabPath_postfix))
                    {
                        mapsData[i * zTileCount + j] = null;
                        onHierarchyIdx = mapTrans.GetSiblingIndex();
                        GameObject.DestroyImmediate(mapTrans.gameObject);
                        mapTrans = null;
                    }

                    //获取后缀名资源不为空 并且对应下标下的mapsData是空的，实例话对应的mapsData，再存入对应下标下
                    if (!string.IsNullOrWhiteSpace(mapPrefabPath_postfix) && mapTrans == null)
                    {
                        GameObject prefab = Resources.Load<GameObject>(mapPrefabPath_prefix + mapPrefabPath_postfix);
                        if (prefab != null)
                        {
                            GameObject obj = Instantiate(prefab, new Vector3(point.x, 0, point.z), Quaternion.Euler(new Vector3(90, 0, 0)), m_transform);
                            mapsData[i * zTileCount + j] = obj.transform;
                            if (onHierarchyIdx > -1) obj.transform.SetSiblingIndex(onHierarchyIdx);
                        }


                    }
                }
            }
        }



        //画x轴方向的辅助线
        for (int i = 0; i < zTileCount; i++)
        {
            Gizmos.color = new Color(0, 0, 1, 1);
            Gizmos.DrawLine(pos + new Vector3(0, pos.y, tileSize * i), transform.TransformPoint(tileSize * xTileCount, pos.y, tileSize * i));
        }
    }

}
