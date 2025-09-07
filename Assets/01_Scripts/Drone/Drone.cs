using UnityEngine;

public class Drone : MonoBehaviour
{
    [SerializeField] CharacterStat stat;
    [SerializeField] GameObject boxPrefab;
    [SerializeField] Transform target;
    [SerializeField] float followSpeed = 3f;
    float timer = 1000;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        timer += Time.deltaTime;
        if (timer >= stat.boxSpawnCoolTime)
        {
            SpawnBox();
            timer = 0;
        }
    }

    private void SpawnBox()
    {
        Instantiate(boxPrefab, transform.position, Quaternion.Euler(Vector3.right * 90f));
    }
}
