public class ObjPoolManager : Singleton<ObjPoolManager> {

    public ObjectPool<BulletType> BulletPool { get; private set; }

    public ObjectPool<EnemyType> EnemyPool { get; private set; }

    public ObjectPool<CollectableType> CollectablePool { get; private set; }
        
    private void Awake() {
        BulletPool = new ObjectPool<BulletType>();
        EnemyPool = new ObjectPool<EnemyType>();
        CollectablePool = new ObjectPool<CollectableType>();
    }
}
