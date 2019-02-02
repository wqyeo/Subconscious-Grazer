using UnityEngine;

[DisallowMultipleComponent]
public class ItemManager : Singleton<ItemManager> {
    [Separator("Item Manager", true)]

    [SerializeField, Tooltip("The respective prefab for the items.")]
    private GameObject[] itemPrefabs;

    public void CreateCollectableAtPos(Vector2 position, ItemType collectableType) {
        var newItemObj = CreateOrFetchCollectablePrefab(collectableType);

        if (newItemObj != null) {
            newItemObj.transform.position = position;
            newItemObj.SetActive(true);
            newItemObj.GetComponent<Item>().InitalizeItem();
        }
    }

    private GameObject CreateOrFetchCollectablePrefab(ItemType collectableType) {
        GameObject newPrefab = ObjPoolManager.Instance.ItemPool.FetchObjByType(collectableType);
        // If no inactive collectable was found in the obj pool.
        if (newPrefab == null) {
            // Instantiate one, and add to the obj pool.
            newPrefab = InstantiateCollectablePrefab(collectableType);
            ObjPoolManager.Instance.ItemPool.AddToObjectPool(collectableType, newPrefab);
        }

        return newPrefab;
    }

    private GameObject InstantiateCollectablePrefab(ItemType collectableType) {
        GameObject instantiatedPrefab = null;

        // Loop through all the prefabs.
        foreach (var itemPrefab in itemPrefabs) {
            // If the prefab matches the one we need to instantiate, instantiate.
            if (itemPrefab.GetComponent<Item>().TypeOfItem == collectableType) {
                instantiatedPrefab = Instantiate(itemPrefab.gameObject);
                break;
            }
        }

        return instantiatedPrefab;
    }
}
