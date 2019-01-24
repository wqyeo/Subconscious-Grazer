using UnityEngine;

public class SpawnDetail : MonoBehaviour {

    [System.Serializable]
    public struct SpawnPoint {
        [Separator("Base details.", true)]

        /// <summary>
        /// The global position of this spawnpoint.
        /// </summary>
        [Tooltip("The global position of this spawnpoint.")]
        public Vector2 position;

        /// <summary>
        /// The enemy prefab to spawn.
        /// </summary>
        [Separator("Enemy details", true)]
        [MustBeAssigned, Tooltip("The enemy prefab to spawn.")]
        public GameObject enemyPrefab;

        [SearchableEnum, Tooltip("The direction of this AI will move to at the start.")]
        public Direction enemyStartDirection;

        /// <summary>
        /// The type of AI action this enemy will perform when spawned.
        /// </summary>
        [SearchableEnum, Tooltip("The type of AI action this enemy will perform when spawned.")]
        public AIType aiType;

        [ConditionalField("aiType", AIType.HitRunAI), Tooltip("How long this AI moves before lingering")]
        public float moveDuration;

        [ConditionalField("aiType", AIType.HitRunAI), Tooltip("How long this AI will stay in the lingering spot before running away. (Negative for stay after move)")]
        public float lingerDuration;

        [ConditionalField("aiType", AIType.HitRunAI), Tooltip("True if this AI only starts shooting after moving.")]
        public bool shootAfterMoving;

        /// <summary>
        /// The delay before the first enemy spawns.
        /// </summary>
        [Separator("Spawning details", true)]
        [Tooltip("The delay before the first enemy spawns.")]
        public float spawnDelay;

        /// <summary>
        /// The amount of enemies to spawn.
        /// </summary>
        [Tooltip("The amount of enemies to spawn.")]
        public int enemyCount;

        /// <summary>
        /// The delay between each enemy spawn.
        /// </summary>
        [Tooltip("The delay between each enemy spawn.")]
        public float enemySpawnDelay;
    }

    [System.Serializable]
    public struct OptionalSpawnPoint {
        [Tooltip("The spawn point and the details of it.")]
        public SpawnPoint spawnPoint;

        [Range(0, 100), Tooltip("Chance for this spawn point to be invoked.")]
        public float spawnInvokedChance;
    }


    [SerializeField, Tooltip("The duration before the spawner invokes another spawn wave.")]
    private float spawnWaveDuration;

    [Header("Spawn Points")]

    [SerializeField, Tooltip("The respective spawnpoints and details of it")]
    private SpawnPoint[] spawnPointsObj;

    [Header("Spawn Points (Optional)")]

    [SerializeField, Tooltip("The respective optional spawn points.")]
    private OptionalSpawnPoint[] optionalSpawnPointsObj;

    #region Properties

    public SpawnPoint[] SpawnPointsObj {
        get {
            return spawnPointsObj;
        }

        set {
            spawnPointsObj = value;
        }
    }

    public float SpawnWaveDuration {
        get {
            return spawnWaveDuration;
        }

        set {
            spawnWaveDuration = value;
        }
    }

    public OptionalSpawnPoint[] OptionalSpawnPointsObj {
        get {
            return optionalSpawnPointsObj;
        }

        set {
            optionalSpawnPointsObj = value;
        }
    }

    #endregion
}
