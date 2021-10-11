using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 处理资源导入
/// </summary>
public class ProcessModel : AssetPostprocessor
{
    /// <summary>
    /// 此方法在有资源导入时自动被调用
    /// </summary>
    /// <param name="input"></param>
    void OnPostprocessModel(GameObject input)
    {
        //处理文件名为Enemy2b的模型
        if (input.name == "Enemy2b")
        {
            //取得导入的模型相关信息
            ModelImporter importer = assetImporter as ModelImporter;

            //将该模型从工程中读出来
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(importer.assetPath);

            GameObject instanceObj = MonoBehaviour.Instantiate(obj, obj.transform.position, obj.transform.rotation);

            //移除不必要的组件
            Animator ani = instanceObj.GetComponent<Animator>();
            if (ani != null) MonoBehaviour.DestroyImmediate(ani);

            //设置tag
            instanceObj.tag = "Enemy";
            //查找碰撞模型
            foreach (Transform trans in instanceObj.GetComponentInChildren<Transform>())
            {
                Debug.Log("foreach =>" + trans);
                if (trans.name == "col")
                {
                    //取消碰撞模型的显示
                    MeshRenderer mr = trans.GetComponent<MeshRenderer>();
                    mr.enabled = false;

                    //添加 Mesh碰撞体
                    if (trans.gameObject.GetComponent<MeshCollider>() == null)
                    {
                        trans.gameObject.AddComponent<MeshCollider>();
                    }

                    //设置碰撞tag
                    trans.tag = "Enemy";
                }
            }

            //设置刚体
            Rigidbody rig = instanceObj.AddComponent<Rigidbody>();
            rig.useGravity = false;//不适用物理重力
            rig.isKinematic = true;

            //添加声音组件
            instanceObj.AddComponent<AudioSource>();

            //获得射击声音 shoot.WAV
            AudioClip shootClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/FX/shoot.WAV");

            //获得子弹的prefab
            GameObject rocket = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/airplane/Prefabs/EnemyRocket.prefab");

            //获得爆炸效果
            GameObject fx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FX/Explosion.prefab");


            //添加敌人脚本
            SuperEnemy enemy = instanceObj.AddComponent<SuperEnemy>();
            enemy.m_life = 5;
            enemy.m_explosionFX = fx.transform;
            enemy.m_shootClip = shootClip;


            //将该模型创建为prefab
            //GameObject prefab = PrefabUtility.CreatePrefab("Assets/Prefabs/Enemy2b.prefab", obj);//过时方法
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(instanceObj, "Assets/airplane/Prefabs/Enemy2b.prefab", out bool isPrefabSuccess);
            MonoBehaviour.DestroyImmediate(instanceObj);

            if (isPrefabSuccess)
            {

                Debug.Log($"资源【{obj.name}】成功制作为预制体");
            }
            else
            {
                Debug.LogError($"资源【{obj.name}】预制体制作失败！");
            }

        }
    }
}
