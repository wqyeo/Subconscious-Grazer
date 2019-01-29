using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> {

    private Dictionary<T, HashSet<GameObject>> objectPools;

    public ObjectPool() {
        objectPools = new Dictionary<T, HashSet<GameObject>>();
    }

    public void ClearAllObjectPool() {
        foreach (var objPool in objectPools) {
            DestroyObjectsInPool(objPool.Value);
        }

        objectPools = new Dictionary<T, HashSet<GameObject>>();
    }

    /// <summary>
    /// Clear an object pool of the given type.
    /// </summary>
    /// <param name="type">The type of the object pool to clear.</param>
    public void ClearObjectPoolOfType(T type) {
        // If the pool does exists.
        if (objectPools.ContainsKey(type)) {
            DestroyObjectsInPool(objectPools[type]);

            objectPools[type] = new HashSet<GameObject>();
        }
    }

    private static void DestroyObjectsInPool(HashSet<GameObject> objPool) {
        foreach (var obj in objPool) {
            Object.Destroy(obj);
        }
    }

    /// <summary>
    /// Add the object to the respective object pool
    /// </summary>
    /// <param name="type">The type of object to add.</param>
    /// <param name="obj">The object to add into the object pool.</param>
    /// <param name="deactiveObj">True to deactive this object when adding to the pool.</param>
    public void AddToObjectPool(T type, GameObject obj, bool deactiveObj = false) {
        // Get the respective object pool.
        var pool = FetchObjectPoolByType(type);
        // Add the object to the pool
        pool.Add(obj);

        if (deactiveObj) {
            obj.SetActive(false);
        }
    }

    /// <summary>
    /// Fetch the first inactive object in the object pool by type.
    /// </summary>
    /// <param name="type">The type of object to fetch.</param>
    /// <returns>An inactive object based on the given type. (Null if none was found)</returns>
    public GameObject FetchObjByType(T type) {
        // Get the respective bullet pool.
        var pool = FetchObjectPoolByType(type);

        return FetchAnyInactiveObjIfExists(pool);
    }

    #region Util

    private static GameObject FetchAnyInactiveObjIfExists(HashSet<GameObject> pool) {
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

    private HashSet<GameObject> FetchObjectPoolByType(T type) {
        // If the pool does not exists yet.
        if (!objectPools.ContainsKey(type)) {
            // Create it
            objectPools.Add(type, new HashSet<GameObject>());
        }

        // Return the pool with given type.
        return objectPools[type];
    }

    #endregion
}
