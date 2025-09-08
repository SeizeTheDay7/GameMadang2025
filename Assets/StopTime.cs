using UnityEngine;
using EditorAttributes;

public class StopTime : MonoBehaviour
{
    [SerializeField] GameObject pannel;
    private void OnEnable()
    {
        pannel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        pannel.SetActive(false);
        Time.timeScale = 1f;
    }
}
