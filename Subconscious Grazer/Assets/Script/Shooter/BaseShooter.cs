using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class BaseShooter : MonoBehaviour {

    [SerializeField, Tooltip("Prefab of the bullet that this shooter shoots out")]
    protected GameObject bulletPrefab;

    [SerializeField, Tooltip("The default speed of all the bullets shot out")]
    protected float bulletSpeed;

    [SerializeField, Tooltip("The default acceleration of all the bullets shot out")]
    protected float bulletAcceleration;

    [SerializeField, Tooltip("The type of the bullet this shooter shoots.")]
    private BulletType bulletType;

    [SerializeField, Tooltip("The sprite of the bullet to change to after fetching a bullet from the object pool.")]
    private Sprite bulletDefaultSprites;

    #region Property

    public float BulletSpeed {
        get {
            return bulletSpeed;
        }
        set {
            bulletSpeed = value;
        }
    }

    public float BulletAcceleration {
        get {
            return bulletAcceleration;
        }
        set {
            bulletAcceleration = value;
        }
    }

    public BulletType BulletType {
        get {
            return bulletType;
        }

        set {
            bulletType = value;
        }
    }

    public Sprite BulletDefaultSprites {
        get {
            return bulletDefaultSprites;
        }

        set {
            bulletDefaultSprites = value;
        }
    }

    #endregion

    public abstract void Shoot();

    protected Bullet InitBullet(Vector2 direction) {
        // Attempt to fetch a bullet from the object pool.
        GameObject newBullet = ObjectPool.Instance.FetchObjectByComponent<Bullet>();

        // If the object pool does not contain a bullet.
        if (newBullet == null) {
            // Instantiate a new bullet.
            newBullet = Instantiate(bulletPrefab);
        } else {
            newBullet.GetComponent<SpriteRenderer>().sprite = bulletDefaultSprites;
            newBullet.SetActive(true);
        }

        // Set the bullet position to this shooter's position.
        newBullet.transform.position = transform.position;

        var bullet = newBullet.GetComponent<Bullet>();

        bullet.Initalize(this, bulletSpeed * direction, bulletAcceleration);

        return bullet;
    }

    private bool IsCorrectBulletType(GameObject bulletObj) {
        bool correctBulletType = false;
        // Get the bullet component from the gameobject.
        var temp = bulletObj.GetComponent<Bullet>();
        // If the component exists.
        if (temp != null) {
            // Check if the bullet type matches this shooter's
            correctBulletType = (temp.Type == bulletType);
        }

        return correctBulletType;
    }
}
