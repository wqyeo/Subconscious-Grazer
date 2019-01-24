using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager> {
    [SerializeField, Tooltip("The spawn details to randomly select from when spawning enemies.")]
    private SpawnDetail[] spawnDetails;

    private float waveTimer;

    private SpawnDetail currentSpawnWave;

    private void Start() {
        SpawnWave();
    }

    private void Update() {
        waveTimer += Time.deltaTime;
        // If this wave timer is up.
        if (currentSpawnWave.SpawnWaveDuration <= waveTimer) {
            // Spawn a new wave.
            SpawnWave();
            // Reset wave timer
            waveTimer = 0f;
        }
    }

    private void SpawnWave() {
        // Generate a random wave number to spawn.
        int genWaveNo = Random.Range(0, spawnDetails.Length - 1);
        // The selected wave.
        var selectedWave = spawnDetails[genWaveNo];
        // Set the current wave to the selected wave.
        currentSpawnWave = selectedWave;
        // Handle the selected wave.
        HandleWave(selectedWave);
    }

    private void HandleWave(SpawnDetail waveDetail) {
        // For each spawnpoint that exists
        foreach (var spawnPoint in waveDetail.SpawnPointsObj) {
            // Handle them
            StartCoroutine(HandleSpawnPoint(spawnPoint));
        }

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

    private IEnumerator HandleSpawnPoint(SpawnDetail.SpawnPoint spawnPoint) {
        // Wait for delay before the first enemy spawn.
        yield return new WaitForSeconds(spawnPoint.spawnDelay);

        // Foreach enemy to spawn
        for (int i = 0; i < spawnPoint.enemyCount; ++i) {
            GameObject newEnemyObj = FetchOrCreateEnemyObject(spawnPoint.enemyPrefab.GetComponent<Enemy>().TypeOfEnemy, spawnPoint.enemyPrefab);
            Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();
            // Move this new enemy to it's spawnpoint
            newEnemyObj.transform.position = spawnPoint.position;

            // Copy the details of the prefab into the new enemy.
            newEnemy.CopyDetails(spawnPoint.enemyPrefab.GetComponent<Enemy>());

            // Set the direction for the enemy respectively.
            newEnemy.MoveDirection = spawnPoint.enemyStartDirection;

            // If this new enemy is going to be a hit-run AI
            if (spawnPoint.aiType == AIType.HitRunAI) {
                // Setup the properties respectively.
                newEnemy.LingerDuration = spawnPoint.lingerDuration;
                newEnemy.ShootAfterMoving = spawnPoint.shootAfterMoving;
                newEnemy.MoveDuration = spawnPoint.moveDuration;
            }
            newEnemy.Invulnerable = true;
            // Initalize this enemy
            newEnemy.InitEnemy(spawnPoint.aiType);

            // Wait for the delay before spawning the next enemy
            yield return new WaitForSeconds(spawnPoint.enemySpawnDelay);
        }

        yield return null;
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
