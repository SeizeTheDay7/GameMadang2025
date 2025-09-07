using UnityEngine;

public class GameOverSceneManager : MonoBehaviour
{
    public void ToLobby()
    {
        SceneChangeManager.Instance.LoadSceneAsync(0);
    }
}
