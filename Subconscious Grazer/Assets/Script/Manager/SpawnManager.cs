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

    [Separator("Debug Properties", true)]

    [SerializeField, Tooltip("True if we only want to spawn bosses")]
    private bool spawnBossOnly;

    private float waveTimer;

    private float chanceToSpawnBoss;

    private SpawnDetail currentSpawnWave;

    public bool BossFight { get; set; }

    private HashSet<Boss> spawnedBosses;
    private SpawnDetail previousWave;

    private void Start() {
        spawnedBosses = new HashSet<Boss>();
        chanceToSpawnBoss = 0f;
        BossFight = false;
        // To spawn a wave immediately.
        currentSpawnWave = spawnDetails[0];
        waveTimer = currentSpawnWave.SpawnWaveDuration;
    }

    private void Update() {
        // We do not want to spawn minions or other bosses during a bossfight
        if (!BossFight) {
            UpdateSpawner(Time.deltaTime);
        }
    }

    private void UpdateSpawner(float deltaTime) {
        waveTimer += deltaTime;
        // If this wave timer is up.
        if (currentSpawnWave.SpawnWaveDuration <= waveTimer) {
            // If the chance to spawn a boss is generated
            // or if we want to spawn bosses only.
            if (Random.Range(0f, 100f) <= chanceToSpawnBoss || spawnBossOnly) {
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
        var bossToSpawn = GetBossToSpawn();
        StartCoroutine(HandleBossSpawning(bossToSpawn));

        // Record down the spawned Boss (So that we do not spawn the same boss again later)
        spawnedBosses.Add(bossToSpawn);

        // A boss is spawned fill up the health bar.
        GameManager.Instance.FillHealthBar();
    }

    private Boss GetBossToSpawn() {
        Boss bossToSpawn = bosses[Random.Range(0, bosses.Length)];
        // If the player already met through all the bosses.
        if (bosses.Length == spawnedBosses.Count) {
            // Clear records of the player meeting the bosses.
            spawnedBosses = new HashSet<Boss>();
        }

        // Ensures that we do not spawn the already met bosses again.
        while (spawnedBosses.Contains(bossToSpawn)) {
            bossToSpawn = bosses[Random.Range(0, bosses.Length)];
        }

        return bossToSpawn;
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
        InitalizeBoss(newBossObj.GetComponent<Boss>());

        yield return null;
    }

    private void InitalizeBoss(Boss bossToInitalize) {
        int bossSpellCount = bossToInitalize.NoOfSpells;
        // If it was set to spawn bosses only in the inspector, let the boss use all the spell-cards.
        // Else, generate a random number of spell cards for the boss to use.
        int bossSpellAmount = spawnBossOnly ? (bossSpellCount - 1) : Random.Range(0, bossSpellCount);

        bossToInitalize.Initalize(bossSpellAmount);
    }

    private void MoveBossFromToByProgress(GameObject bossObj, float from, float to, float progress) {
        var temp = bossObj.transform.position;
        temp.y = Mathf.Lerp(from, to, progress);
        bossObj.transform.position = temp;
    }

    private void SpawnWave() {
        // Set the current wave to the selected wave.
        currentSpawnWave = GetNextWaveToSpawn();
        // Handle the selected wave.
        HandleWave(currentSpawnWave);
        previousWave = currentSpawnWave;
    }

    private SpawnDetail GetNextWaveToSpawn() {
        int genWaveNo = Random.Range(0, spawnDetails.Length);
        SpawnDetail waveToSpawn = spawnDetails[genWaveNo];

        // If there was a wave before this.
        if (previousWave != null) {
            // Ensure that we do not select the previous wave again.
            while (previousWave == waveToSpawn) {
                genWaveNo = Random.Range(0, spawnDetails.Length);
                waveToSpawn = spawnDetails[genWaveNo];
            }
        }

        return waveToSpawn;
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
