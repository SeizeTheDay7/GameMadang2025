using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public static SceneChangeManager Instance { get; private set; }

    [SerializeField] private string loadingSceneName = "LoadingScene";

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneAsync(int index)
    {
        SceneManager.LoadScene(index);
        if (index == 1)
        {
            StartCoroutine(LoadUIScene());
            return;
        }
    }
    public void LoadSceneAsync(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        if (sceneName != "LobbyScene" && sceneName != "GameOver")
        {
            StartCoroutine(LoadUIScene());
            return;
        }
    }

    IEnumerator LoadUIScene()
    {
        AsyncOperation uiSceneOp = SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);
        uiSceneOp.allowSceneActivation = true;
        while (!uiSceneOp.isDone)
        {
            yield return null;
        }
    }
}
