using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    // 로딩 씬 이름을 지정하세요 (예: "LoadingScene")
    [SerializeField] private string loadingSceneName = "LoadingScene";

    public void LoadSceneAsync(int index)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(index));
    }

    private IEnumerator LoadSceneAsyncCoroutine(int index)
    {
        // 1. 로딩 씬 먼저 로드 (싱글 모드로 전환)
        AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);
        while (!loadingSceneOp.isDone)
        {
            yield return null;
        }

        // 2. 실제 로드할 씬 비동기로 로드 (Additive 모드)
        AsyncOperation targetSceneOp = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        targetSceneOp.allowSceneActivation = false;

        // 3. 로딩 진행률 체크 (로딩씬에서 진행률 표시 가능)
        while (targetSceneOp.progress < 0.9f)
        {
            // 필요시 진행률 전달
            yield return null;
        }

        // 4. 실제 씬 활성화
        targetSceneOp.allowSceneActivation = true;
        while (!targetSceneOp.isDone)
        {
            yield return null;
        }

        // 5. 로딩씬 언로드
        SceneManager.UnloadSceneAsync(loadingSceneName);

        // 6. 타겟 씬을 활성화 (필요시)
        Scene targetScene = SceneManager.GetSceneByBuildIndex(index);
        if (targetScene.IsValid())
        {
            SceneManager.SetActiveScene(targetScene);
        }
    }
}
