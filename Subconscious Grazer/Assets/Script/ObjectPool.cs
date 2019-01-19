using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool : Singleton<ObjectPool> {

    private HashSet<GameObject> objectPool;

    private void Awake() {
        objectPool = new HashSet<GameObject>();
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

        HashSet<GameObject> temp = new HashSet<GameObject>();

        // For counting how many objects we already have in the temporary list.
        int i = 0;
        // Go through the object pool.
        foreach (var obj in objectPool) {
            // If we have already reached the max array size.
            if (i >= maxSize) {
                // Exit loop.
                break;
            }
            // If the current object contains the desired component.
            else if (obj.GetComponent<T>() != null) {
                // Add to the temporary list.
                temp.Add(obj);
                // An object has been added.
                ++i;
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
        HashSet<GameObject> temp = new HashSet<GameObject>();

        // For counting how many objects we already have in the temporary list.
        int i = 0;
        // Go through the object pool.
        foreach (var obj in objectPool) {
            // If we have already reached the max array size.
            if (i >= maxSize) {
                // Exit loop.
                break;
            }
            // If the current object meets the condition.
            else if (condition(obj)) {
                // Add to the temporary list.
                temp.Add(obj);
                // An object has been added.
                ++i;
            }
        }

        var fetchedObjects = temp.ToArray();

        // If we need to remove the fetched objects from the object pool
        if (removeFromPool) {
            RemoveObjectsFromPool(fetchedObjects);
        }

        return fetchedObjects;
    }

    /// <summary>
    /// Fetch a gameobject with the given condition.
    /// </summary>
    /// <param name="condition">The condition based on to fetch the gameobject</param>
    /// <param name="removeFromPool">True to remove this gameobject from the object pool.</param>
    /// <returns>The fetched gameobject by the respective condition. (Null if none is found.)</returns>
    public GameObject FetchObjectByCondition(Func<GameObject, bool> condition, bool removeFromPool = true) {

        GameObject fetchedObject = null;

        // Loop through the object pool.
        foreach (var obj in objectPool) {
            // If this object's condition meets the given condition.
            if (condition(obj)) {
                // Fetch this object.
                fetchedObject = obj;
                // Stop loop (object is found.)
                break;
            }
        }

        // Remove this object pool if required.
        if (removeFromPool && fetchedObject != null){
            objectPool.Remove(fetchedObject);
        }

        return fetchedObject;
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
