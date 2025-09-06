using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    // �ε� �� �̸��� �����ϼ��� (��: "LoadingScene")
    [SerializeField] private string loadingSceneName = "LoadingScene";

    public void LoadSceneAsync(int index)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(index));
    }

    private IEnumerator LoadSceneAsyncCoroutine(int index)
    {
        // 1. �ε� �� ���� �ε� (�̱� ���� ��ȯ)
        AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Single);
        while (!loadingSceneOp.isDone)
        {
            yield return null;
        }

        // 2. ���� �ε��� �� �񵿱�� �ε� (Additive ���)
        AsyncOperation targetSceneOp = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        targetSceneOp.allowSceneActivation = false;

        // 3. �ε� ����� üũ (�ε������� ����� ǥ�� ����)
        while (targetSceneOp.progress < 0.9f)
        {
            // �ʿ�� ����� ����
            yield return null;
        }

        // 4. ���� �� Ȱ��ȭ
        targetSceneOp.allowSceneActivation = true;
        while (!targetSceneOp.isDone)
        {
            yield return null;
        }

        // 5. �ε��� ��ε�
        SceneManager.UnloadSceneAsync(loadingSceneName);

        // 6. Ÿ�� ���� Ȱ��ȭ (�ʿ��)
        Scene targetScene = SceneManager.GetSceneByBuildIndex(index);
        if (targetScene.IsValid())
        {
            SceneManager.SetActiveScene(targetScene);
        }
    }
}
