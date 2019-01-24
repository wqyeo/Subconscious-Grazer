using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool> {

    private Dictionary<BulletType, HashSet<GameObject>> bulletPools;

    private void Awake() {
        bulletPools = new Dictionary<BulletType, HashSet<GameObject>>();
    }

    /// <summary>
    /// Add the bullet to the bullet pool
    /// </summary>
    /// <param name="type">The type of bullet to add.</param>
    /// <param name="bulletObj">The bullet object.</param>
    /// <param name="deactiveObj">True to deactive this object when adding to the pool.</param>
    public void AddToBulletPool(BulletType type, GameObject bulletObj, bool deactiveObj = false) {
        // Get the respective bullet pool.
        var pool = FetchPoolByType(type);
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
        var pool = FetchPoolByType(type);

        GameObject fetchObj = null;

        // Loop through the pool
        foreach (var obj in pool) {
            // If this bullet object is not active.
            if (!obj.activeInHierarchy) {
                // Fetch this object
                fetchObj = obj;
                // Stop loop, a suitable bullet is found
                break;
            }
        }

        return fetchObj;
    }

    #region Util

    private HashSet<GameObject> FetchPoolByType(BulletType type) {
        // If the pool does not exists yet.
        if (!bulletPools.ContainsKey(type)) {
            // Create it.
            bulletPools.Add(type, new HashSet<GameObject>());
        }

        // Fetch the bullet pool with the given type
        return bulletPools[type];
    }

    #endregion
}
