using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager> {
    [SerializeField, Tooltip("The spawn details to randomly select from when spawning enemies.")]
    private SpawnDetail[] spawnDetails;

    [SerializeField, Tooltip("The bosses that are spawnable.")]
    private Boss[] bosses;

    [SerializeField, Tooltip("The chance increase to spawn a boss whenever a boss is not spawned.")]
    private float stackChanceAmt;

    private float waveTimer;

    private float chanceToSpawnBoss;

    private SpawnDetail currentSpawnWave;

    public bool BossFight { get; set; }

    private void Start() {
        chanceToSpawnBoss = 0f;
        BossFight = false;
        SpawnWave();
    }

    private void Update() {
        waveTimer += Time.deltaTime;

        if (!BossFight) {
            UpdateSpawner();
        }
    }

    private void UpdateSpawner() {
        // If this wave timer is up.
        if (currentSpawnWave.SpawnWaveDuration <= waveTimer) {
            // If the chance to spawn a boss is generated, spawn.
            if (Random.Range(0, 100) <= chanceToSpawnBoss) {
                BossFight = true;
                chanceToSpawnBoss = 0f;
                SpawnBoss();
            } else {
                
                SpawnWave();
                chanceToSpawnBoss += stackChanceAmt;
            }
            // Reset wave timer
            waveTimer = 0f;
        }
    }

    private void SpawnBoss() {
        var bossToSpawn = bosses[Random.Range(0, bosses.Length)];
        StartCoroutine(HandleBossSpawning(bossToSpawn));
    }

    private IEnumerator HandleBossSpawning(Boss bossToSpawn) {
        var newBossObj = Instantiate(bossToSpawn.gameObject, new Vector2(0, 8), Quaternion.identity);

        float from = 8;
        float to = 3.25f;
        float progress = 0f;

        // Progressively move the boss to where it should be.
        while (progress <= 1) {
            MoveBossFromToByProgress(newBossObj, from, to, progress);

            progress += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        ObjPoolManager.Instance.EnemyPool.ClearInactiveObjectsInPools();
        ObjPoolManager.Instance.BulletPool.ClearInactiveObjectsInPools();

        newBossObj.GetComponent<Boss>().Initalize(Random.Range(0, newBossObj.GetComponent<Boss>().NoOfSpells));

        yield return null;
    }

    private void MoveBossFromToByProgress(GameObject bossObj, float from, float to, float progress) {
        var temp = bossObj.transform.position;
        temp.y = Mathf.Lerp(from, to, progress);
        bossObj.transform.position = temp;
    }

    private void SpawnWave() {
        // Set the current wave to the selected wave.
        currentSpawnWave = RandomlySelectWave();
        // Handle the selected wave.
        HandleWave(currentSpawnWave);
    }

    private SpawnDetail RandomlySelectWave() {
        int genWaveNo = Random.Range(0, spawnDetails.Length);
        return spawnDetails[genWaveNo];
    }

    private void HandleWave(SpawnDetail waveDetail) {
        HandleRequiredSpawnPoints(waveDetail);

        HandleOptionalSpawnPoints(waveDetail);
    }

    private void HandleOptionalSpawnPoints(SpawnDetail waveDetail) {
        // Foreach optional spawnpoint that exists
        foreach (var opSpawnPoint in waveDetail.OptionalSpawnPointsObj) {
            var chance = Random.Range(0, 100);
            // If the chance to spawn them has occured
            if (chance <= opSpawnPoint.spawnInvokedChance) {
                // Handle spawning.
                StartCoroutine(HandleSpawnPoint(opSpawnPoint.spawnPoint));
            }
        }
    }

    private void HandleRequiredSpawnPoints(SpawnDetail waveDetail) {
        // For each spawnpoint that exists
        foreach (var spawnPoint in waveDetail.SpawnPointsObj) {
            // Handle them
            StartCoroutine(HandleSpawnPoint(spawnPoint));
        }
    }

    private IEnumerator HandleSpawnPoint(SpawnDetail.SpawnPoint spawnPoint) {
        // Wait for delay before the first enemy spawn.
        yield return new WaitForSeconds(spawnPoint.spawnDelay);

        // Foreach enemy to spawn
        for (int i = 0; i < spawnPoint.enemyCount; ++i) {
            SpawnEnemyOnSpawnPoint(spawnPoint);

            // Wait for the delay before spawning the next enemy
            yield return new WaitForSeconds(spawnPoint.enemySpawnDelay);
        }

        yield return null;
    }

    private void SpawnEnemyOnSpawnPoint(SpawnDetail.SpawnPoint spawnPoint) {
        GameObject newEnemyObj = FetchOrCreateEnemyObject(spawnPoint.enemyPrefab.GetComponent<Enemy>().TypeOfEnemy, spawnPoint.enemyPrefab);
        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();
        // Move this new enemy to it's spawnpoint
        newEnemyObj.transform.position = spawnPoint.position;

        // Copy the details of the prefab into the new enemy.
        newEnemy.CopyDetails(spawnPoint.enemyPrefab.GetComponent<Enemy>());

        SetEnemyAIBySpawnPoint(newEnemy, spawnPoint);

        // Initalize this enemy
        newEnemy.InitEnemy(spawnPoint.aiType);

        newEnemy.Invulnerable = true;
    }

    private void SetEnemyAIBySpawnPoint(Enemy enemy, SpawnDetail.SpawnPoint spawnPoint) {
        // Set the direction for the enemy respectively.
        enemy.MoveDirection = spawnPoint.enemyStartDirection;

        // If this new enemy is going to be a hit-run AI
        if (spawnPoint.aiType == AIType.HitRunAI) {
            // Setup the properties respectively.
            enemy.LingerDuration = spawnPoint.lingerDuration;
            enemy.ShootAfterMoving = spawnPoint.shootAfterMoving;
            enemy.MoveDuration = spawnPoint.moveDuration;
        }
    }

    private GameObject FetchOrCreateEnemyObject(EnemyType enemyType, GameObject enemyObj) {
        // Fetch an enemy of the similar type from the object pool.
        var enemy = ObjPoolManager.Instance.EnemyPool.FetchObjByType(enemyType);
        // If there is no avaiable enemy in the object pool.
        if (enemy == null) {
            // Create one.
            enemy = Instantiate(enemyObj);
            // Add it to the object pool
            ObjPoolManager.Instance.EnemyPool.AddToObjectPool(enemyType, enemy);
        }

        // Return the fetched gameobject
        return enemy;
    }
}
