using EditorAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    [SerializeField] private string loadingSceneName = "LoadingScene";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneAsync(int index)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(index));
    }

    private IEnumerator LoadSceneAsyncCoroutine(int index)
    {
        AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);
        while (!loadingSceneOp.isDone)
        {
            yield return null;
        }

        AsyncOperation targetSceneOp = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        targetSceneOp.allowSceneActivation = false;

        while (targetSceneOp.progress < 0.9f)
        {
            print(targetSceneOp.progress);
            yield return null;
        }

        targetSceneOp.allowSceneActivation = true;
        while (!targetSceneOp.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(loadingSceneName);

        Scene targetScene = SceneManager.GetSceneByBuildIndex(index);
        if (targetScene.IsValid())
        {
            SceneManager.SetActiveScene(targetScene);
        }
    }
}
