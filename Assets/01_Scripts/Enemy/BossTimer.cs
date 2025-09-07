using TMPro;
using UnityEngine;

public class BossTimer : MonoBehaviour
{
    public float timelimit = 360f;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] GameObject boss;

    void Update()
    {
        timelimit -= Time.deltaTime;
        if (timelimit <= 0)
        {
            timer.text = "";
            Instantiate(boss, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        timer.text = $"Boss : {Mathf.FloorToInt(timelimit / 60):00}:{Mathf.FloorToInt(timelimit % 60):00}";
    }
}