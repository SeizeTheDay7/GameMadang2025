using System.IO;
using EditorAttributes;
using UnityEngine;

public class FlyingEnemySpawner : MonoBehaviour
{
    [SerializeField] CharacterStat stat;

    [SerializeField] FlyingEnemy enemy1small;
    [SerializeField] FlyingEnemy enemy1middle;
    [SerializeField] FlyingEnemy enemy1big;

    [SerializeField] FlyingEnemy enemy2small;
    [SerializeField] FlyingEnemy enemy2middle;
    [SerializeField] FlyingEnemy enemy2big;
    [SerializeField] FlyingEnemy enemy3small;
    [SerializeField] FlyingEnemy enemy3middle;
    [SerializeField] FlyingEnemy enemy3big;

    float spawnInterval;
    [SerializeField] float spawnInterval1 = 7.5f; // 6, 4.5
    [SerializeField] float spawnInterval2 = 5.5f;
    [SerializeField] float spawnInterval3 = 3.5f;
    float spawnTimer = 0f;
    [SerializeField] Transform[] spawnTransforms;
    [SerializeField] Attributes character;

    private void Start()
    {
        spawnInterval = spawnInterval1;
        spawnTimer = 0;
    }
    private void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            Spwan();
            spawnTimer = spawnInterval1;
            if (stat.currentLevel > 15f) spawnInterval = spawnInterval3;
            else if (stat.currentLevel > 7f) spawnInterval = spawnInterval2;
            else spawnInterval = spawnInterval1;
        }
    }
    [Button]
    public void Spwan()
    {
        if (spawnTransforms.Length == 0 || !character) return;
        Transform spawnPoint = spawnTransforms[Random.Range(0, spawnTransforms.Length)];
        // {enemy1 small 스폰 확률, 1 middle, 1 big, 2 small, 2 middle, 2 big, 3 small, 3 middle, 3 big}
        // 레벨 7 이하면 enemy1들만 {50,30,20}
        // 레벨 8 이상이면 enemy2들만 {50,30,20}
        // 레벨 16 이상이면 enemy3들만 {50,30,20}
        int level = stat.currentLevel;
        FlyingEnemy enemyPrefab = null;
        int rand = Random.Range(0, 100);

        if (level <= 7)
        {
            if (rand < 50) enemyPrefab = enemy1small;
            else if (rand < 80) enemyPrefab = enemy1middle;
            else enemyPrefab = enemy1big;
        }
        else if (level <= 15)
        {
            if (rand < 50) enemyPrefab = enemy2small;
            else if (rand < 80) enemyPrefab = enemy2middle;
            else enemyPrefab = enemy2big;
        }
        else
        {
            if (rand < 50) enemyPrefab = enemy3small;
            else if (rand < 80) enemyPrefab = enemy3middle;
            else enemyPrefab = enemy3big;
        }

        FlyingEnemy newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.Euler(90, 0, 0));
        newEnemy.SetCharacter(character);
    }
}
