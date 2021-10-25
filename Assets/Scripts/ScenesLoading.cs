using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesLoading : MonoBehaviour
{
    public static ScenesLoading Instance;

    public Image m_IconComponent;
    public Sprite[] m_IconSprite;
    public Animator[] m_anis;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// 开始切换场景
    /// </summary>
    /// <param name="currScene">当前场景</param>
    /// <param name="targetSceneIndex">目标场景的index</param>
    public void StartLoading(Scene currScene, int targetSceneIndex)
    {
        if (!this.gameObject.activeSelf) this.gameObject.SetActive(true);
        SceneManager.UnloadScene(currScene);
        StartCoroutine(DoScenesFadeIn(targetSceneIndex));
    }


    /// <summary>
    /// 异步加载场景，执行淡入淡出动画
    /// </summary>
    /// <param name="sceneIndex">目标场景index</param>
    /// <returns></returns>
    IEnumerator DoScenesFadeIn(int sceneIndex)
    {
        yield return null;
        for (int i = 0; i < m_anis.Length; i++)
        {
            m_anis[i].Play("ScenesBgFadeOut");
        }

        yield return new WaitForSeconds(0.5f);
        AsyncOperation aync = SceneManager.LoadSceneAsync(sceneIndex);


        int idx = 0;
        while (aync.progress < 0.9f)
        {
            yield return new WaitForSeconds(0.5f);
            m_IconComponent.sprite = m_IconSprite[idx];

            if (idx == 0)
                idx = 1;
            else if (idx == 1)
                idx = 0;

        }

        for (int i = 0; i < m_anis.Length; i++)
        {
            m_anis[i].Play("ScenesBgFadeIn");
        }

        yield break;

    }


    /// <summary>
    /// 退出程序
    /// </summary>
    private void OnApplicationQuit()
    {
        Instance = null;
        System.GC.Collect();
    }
}
