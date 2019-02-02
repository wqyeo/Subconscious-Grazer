using System.Collections;
using System.Collections.Generic;

public class ObjPoolManager : Singleton<ObjPoolManager> {

    public ObjectPool<BulletType> BulletPool { get; private set; }

    public ObjectPool<EnemyType> EnemyPool { get; private set; }

    public ObjectPool<ItemType> ItemPool { get; private set; }
        
    private void Awake() {
        BulletPool = new ObjectPool<BulletType>();
        EnemyPool = new ObjectPool<EnemyType>();
        ItemPool = new ObjectPool<ItemType>();
    }
}
