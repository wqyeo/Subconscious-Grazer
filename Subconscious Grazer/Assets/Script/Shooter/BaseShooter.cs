using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseShooter : MonoBehaviour {

    public delegate void OnBulletDestroyed(Bullet destroyedBullet);

    public OnBulletDestroyed onBulletDestroy;

    [Header("Base shooter properties")]

    [SerializeField, Tooltip("Prefab of the bullet that this shooter shoots out")]
    protected GameObject bulletPrefab;

    [SerializeField, Tooltip("The default speed of all the bullets shot out")]
    protected float bulletSpeed;

    [SerializeField, Tooltip("The default acceleration of all the bullets shot out")]
    protected float bulletAcceleration;

    [SerializeField, Tooltip("The type of the bullet this shooter shoots.")]
    private BulletType bulletType;

    [Range(0, 360), SerializeField, Tooltip("How much more to rotate the bullet by. (For the sprite to show correctly.)")]
    private float bulletRotationOffset;

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

    /// <summary>
    /// The sprite of the bullet to change to after fetching a bullet from the object pool.
    /// </summary>
    public Sprite BulletDefaultSprites {
        get; set;
    }

    public float BulletRotationOffset {
        get {
            return bulletRotationOffset;
        }

        set {
            bulletRotationOffset = value;
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
            newBullet.SetActive(true);
        }

        if (BulletDefaultSprites != null) {
            // Set the bullet sprite.
            newBullet.GetComponent<SpriteRenderer>().sprite = BulletDefaultSprites;
        }

        // Rotate the bullet respectively.
        var tempRotation = newBullet.transform.rotation.eulerAngles;
        tempRotation.z += bulletRotationOffset;
        newBullet.transform.eulerAngles = tempRotation;

        // Set the bullet position to this shooter's position.
        newBullet.transform.position = transform.position;

        var bullet = newBullet.GetComponent<Bullet>();

        bullet.Initalize(this, bulletSpeed * direction, bulletAcceleration);

        // If there are listeners to add.
        if (onBulletDestroy != null) {
            // Add them.
            bullet.OnBulletDestroyedEvent += OnBulletDestroyedEvent;
        }
        

        return bullet;
    }

    private void OnBulletDestroyedEvent(object sender, EventArgs e) {
        // If the delegate still exists.
        if (onBulletDestroy != null) {
            // Invoke delegate.
            onBulletDestroy(sender as Bullet);
        }
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

    public void AddOnBulletDestroyedListener(OnBulletDestroyed listener) {
        if (onBulletDestroy == null) {
            onBulletDestroy = listener;
        } else {
            onBulletDestroy += listener;
        }
    }

    public void ClearAllOnBulletDestroyedListener() {
        onBulletDestroy = null;
    }
}
