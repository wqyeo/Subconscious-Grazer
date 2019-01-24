using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool> {

    private Dictionary<BulletType, HashSet<GameObject>> bulletPools;
    private Dictionary<EnemyType, HashSet<GameObject>> enemyPools;

    private void Awake() {
        bulletPools = new Dictionary<BulletType, HashSet<GameObject>>();
        enemyPools = new Dictionary<EnemyType, HashSet<GameObject>>();
    }

    #region BulletPool

    /// <summary>
    /// Add the bullet to the bullet pool
    /// </summary>
    /// <param name="type">The type of bullet to add.</param>
    /// <param name="bulletObj">The bullet object.</param>
    /// <param name="deactiveObj">True to deactive this object when adding to the pool.</param>
    public void AddToBulletPool(BulletType type, GameObject bulletObj, bool deactiveObj = false) {
        // Get the respective bullet pool.
        var pool = FetchBulletPoolByType(type);
        // Add the bullet to the pool
        pool.Add(bulletObj);

        // If we need to deactive this object
        if (deactiveObj) {
            bulletObj.SetActive(false);
        }
    }

    /// <summary>
    /// Fetch the first inactive bullet object in the object pool by bullet type.
    /// </summary>
    /// <param name="type">The type of bullet to fetch.</param>
    /// <returns>An inactive bullet object based on the given bullet-type. (Null if none was found)</returns>
    public GameObject FetchBulletObjByType(BulletType type) {
        // Get the respective bullet pool.
        var pool = FetchBulletPoolByType(type);

        return FetchAnyInactiveObjIfExists(pool);
    }

    #endregion

    #region EnemyPool

    /// <summary>
    /// Add the enemy to the enemy pool
    /// </summary>
    /// <param name="type">The type of enemy to add.</param>
    /// <param name="enemyObj">The enemy object.</param>
    /// <param name="deactiveObj">True to deactive this object when adding to the pool.</param>
    public void AddToEnemyPool(EnemyType type, GameObject enemyObj, bool deactiveObj = false) {
        // Get the respective enemy pool.
        var pool = FetchEnemyPoolByType(type);
        // Add the enemy to the pool
        pool.Add(enemyObj);

        // If we need to deactive this object
        if (deactiveObj) {
            enemyObj.SetActive(false);
        }
    }

    /// <summary>
    /// Fetch the first inactive enemy object in the object pool by enemy type.
    /// </summary>
    /// <param name="type">The type of enemy to fetch.</param>
    /// <returns>An inactive enemy object based on the given enemy-type. (Null if none was found)</returns>
    public GameObject FetchEnemyObjByType(EnemyType type) {
        // Get the respective enemy pool.
        var pool = FetchEnemyPoolByType(type);

        return FetchAnyInactiveObjIfExists(pool);
    }

    #endregion

    #region Util

    private GameObject FetchAnyInactiveObjIfExists(HashSet<GameObject> pool) {
        GameObject fetchObj = null;

        // Loop through the pool
        foreach (var obj in pool) {
            // If this object is not active.
            if (!obj.activeInHierarchy) {
                // Fetch this object
                fetchObj = obj;
                // Stop loop, an inactive object is fetched.
                break;
            }
        }

        return fetchObj;
    }

    private HashSet<GameObject> FetchBulletPoolByType(BulletType type) {
        // If the pool does not exists yet.
        if (!bulletPools.ContainsKey(type)) {
            // Create it.
            bulletPools.Add(type, new HashSet<GameObject>());
        }

        // Fetch the bullet pool with the given type
        return bulletPools[type];
    }

    private HashSet<GameObject> FetchEnemyPoolByType(EnemyType type) {
        // If the pool does not exists yet.
        if (!enemyPools.ContainsKey(type)) {
            // Create it.
            enemyPools.Add(type, new HashSet<GameObject>());
        }

        // Fetch the bullet pool with the given type
        return enemyPools[type];
    }

    #endregion
}
