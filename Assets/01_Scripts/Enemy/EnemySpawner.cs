using EditorAttributes;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] EnemyBase enemyPrefab;
    [SerializeField] float spawnInterval = 2f;
    float spawnTimer = 0f;
    [SerializeField] Transform[] spawnTransforms;
    [SerializeField] Attributes character;

    private void Start()
    {
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            Spwan();
            spawnTimer = spawnInterval;
        }
    }

    [Button]
    public void Spwan()
    {
        if (spawnTransforms.Length == 0 || !enemyPrefab || !character) return;
        Transform spawnPoint = spawnTransforms[Random.Range(0, spawnTransforms.Length)];
        EnemyBase newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.Euler(90,0,0));
        newEnemy.SetCharacter(character);
    }
}
