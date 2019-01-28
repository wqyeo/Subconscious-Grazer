using UnityEngine;

[DisallowMultipleComponent]
public class CollectableManager : Singleton<CollectableManager> {
    [Separator("Collectable Manager", true)]

    [SerializeField, Tooltip("The respective prefab for the collectables.")]
    private GameObject[] collectablePrefabs;

    public void CreateCollectableAtPos(Vector2 position, CollectableType collectableType) {
        var newCollectable = CreateOrFetchCollectablePrefab(collectableType);
        if (newCollectable != null) {
            newCollectable.transform.position = position;
            newCollectable.SetActive(true);
        }
    }

    private GameObject CreateOrFetchCollectablePrefab(CollectableType collectableType) {
        GameObject newPrefab = ObjPoolManager.Instance.CollectablePool.FetchObjByType(collectableType);

        if (newPrefab == null) {
            newPrefab = InstantiateCollectablePrefab(collectableType);
            ObjPoolManager.Instance.CollectablePool.AddToObjectPool(collectableType, newPrefab);
        }

        return newPrefab;
    }

    private GameObject InstantiateCollectablePrefab(CollectableType collectableType) {
        GameObject instaniatedPrefab = null;

        foreach (var collectablePrefab in collectablePrefabs) {
            if (collectablePrefab.GetComponent<Collectable>().TypeOfCollectable == collectableType) {
                instaniatedPrefab = Instantiate(collectablePrefab.gameObject);
                break;
            }
        }

        return instaniatedPrefab;
    }
}
