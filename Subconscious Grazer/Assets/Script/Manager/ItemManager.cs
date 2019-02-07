using UnityEngine;

[DisallowMultipleComponent]
public class ItemManager : Singleton<ItemManager> {
    [Separator("Item Manager", true)]

    [SerializeField, Tooltip("The respective prefab for the items.")]
    private GameObject[] itemPrefabs;

    public void CreateCollectableAtPos(Vector2 position, ItemType itemType) {
        var newItemObj = CreateOrFetchItemPrefabIfExists(itemType);

        // Do nothing if the item does not exists
        if (newItemObj != null) {
            newItemObj.transform.position = position;
            newItemObj.SetActive(true);
            newItemObj.GetComponent<Item>().InitalizeItem();
        }
    }

    private GameObject CreateOrFetchItemPrefabIfExists(ItemType itemType) {
        GameObject newPrefab = ObjPoolManager.Instance.ItemPool.FetchObjByType(itemType);
        // If no inactive collectable was found in the obj pool.
        if (newPrefab == null) {
            // Instantiate one, and add to the obj pool.
            newPrefab = InstantiateItemPrefab(itemType);
            ObjPoolManager.Instance.ItemPool.AddToObjectPool(itemType, newPrefab);
        }

        return newPrefab;
    }

    private GameObject InstantiateItemPrefab(ItemType itemType) {
        GameObject instantiatedPrefab = null;

        // Loop through all the prefabs.
        foreach (var itemPrefab in itemPrefabs) {
            // If the prefab matches the one we need to instantiate, instantiate.
            if (itemPrefab.GetComponent<Item>().TypeOfItem == itemType) {
                instantiatedPrefab = Instantiate(itemPrefab.gameObject);
                break;
            }
        }

        return instantiatedPrefab;
    }
}
