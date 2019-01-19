using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool : Singleton<ObjectPool> {

    private List<GameObject> objectPool;

    private void Awake() {
        objectPool = new List<GameObject>();
    }

    /// <summary>
    /// Add a gameobject to the object pool.
    /// </summary>
    /// <param name="objToPool">The gameobject to add to the object pool.</param>
    /// <param name="deactivateObj">Deactivate this gameobject upon storing into the object pool.</param>
    public void AddToPool(GameObject objToPool, bool deactivateObj = true) {
        objectPool.Add(objToPool);

        // If we need to deactivate this gameobject.
        if (deactivateObj) {
            // Set it to not active.
            objToPool.SetActive(false);
        }
    }

    /// <summary>
    /// Fetch a gameobject from the pool, with the gameobject containing the desired component.
    /// </summary>
    /// <typeparam name="T">The type of the component.</typeparam>
    /// <param name="removeFromPool">True if the fetched gameobject needs to be removed from the pool.</param>
    /// <returns>The respective gameobject. (Null if none is found)</returns>
    public GameObject FetchObjectByComponent<T>(bool removeFromPool = true) where T : MonoBehaviour {
        GameObject fetchedObject = null;

        // Foreach game object in the object pool
        foreach (var gameObj in objectPool) {
            // If this gameobject has the desired component.
            if (gameObj.GetComponent<T>() != null) {
                // Fetch this object.
                fetchedObject = gameObj;
                // End loop (an object with the desired component is found.)
                break;
            }
        }

        // If an object is fetched and we need to remove it from the pool.
        if (fetchedObject != null && removeFromPool) {
            // Remove the fetched object from the pool.
            objectPool.Remove(fetchedObject);
        }

        return fetchedObject;
    }

    /// <summary>
    /// Fetch an array of gameobjects that contains the desired component.
    /// </summary>
    /// <typeparam name="T">The type of the component the gameobject must contain.</typeparam>
    /// <param name="maxSize">The max size of the array returned. (Negative for limitless)</param>
    /// <param name="removeFromPool">True to remove the respective fetched gameobject from the object pool.</param>
    /// <returns>The respective fetched game objects.</returns>
    public GameObject[] FetchObjectsByComponent<T>(int maxSize = -1, bool removeFromPool = true) where T : MonoBehaviour {

        List<GameObject> temp = new List<GameObject>();

        // Loop through the object pool as long as the size limit it not reached.
        for (int i = 0; i < objectPool.Count && i < maxSize; ++i) {
            // If this current object contains the desired component.
            if (objectPool[i].GetComponent<T>() != null) {
                // Add to the temporary list
                temp.Add(objectPool[i]);
            }
        }

        var fetchedObjects = temp.ToArray();

        // If we need to remove the fetched objects from the object pool, remove.
        if (removeFromPool) {
            RemoveObjectsFromPool(fetchedObjects);
        }

        return fetchedObjects;
    }

    /// <summary>
    /// Fetch an array of gameobject based on the given condition.
    /// </summary>
    /// <param name="condition">The condition to check on when fetching gameobjects.</param>
    /// <param name="maxSize">The maximum size of the array returned. (Negative for limitless.)</param>
    /// <param name="removeFromPool">True to remove the respective fetched gameobject from the object pool.</param>
    /// <returns>The respective fetched game objects.</returns>
    public GameObject[] FetchObjectsByCondition(Func<GameObject, bool> condition, int maxSize = -1, bool removeFromPool = true) {
        // Fetch all the matching objects.
        var fetchedObjects = objectPool.Where(condition).ToArray();

        // If an array size limit is given.
        if (maxSize >= 1) {
            List<GameObject> temp = new List<GameObject>();

            // Loop through the fetched objects, adding to the list as long as the list stays in it's given size limit.
            for (int i = 0; i < fetchedObjects.Length && i < maxSize; ++i) {
                temp.Add(fetchedObjects[i]);
            }

            fetchedObjects = temp.ToArray();
        }

        // If we need to remove the fetched objects from the object pool
        if (removeFromPool) {
            RemoveObjectsFromPool(fetchedObjects);
        }

        return fetchedObjects;
    }

    #region Util

    private void RemoveObjectsFromPool(GameObject[] objectsToRemove) {
        // For each given object.
        foreach (var gameObject in objectsToRemove) {
            // Remove the given object from the object pool.
            objectPool.Remove(gameObject);
        }
    }

    #endregion
}
