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

    public BulletType ShotBulletType {
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
    public Sprite BulletDefaultSprites { get; set; }

    public float BulletRotationOffset {
        get {
            return bulletRotationOffset;
        }

        set {
            bulletRotationOffset = value;
        }
    }

    #endregion

    /// <summary>
    /// Shoot a bullet, defining if the bullet should constantly rotate to the direction it is travelling to.
    /// </summary>
    /// <param name="rotateBulletToDirection">True if the bullet should constantly rotate to the direction it is travelling to.</param>
    public abstract void Shoot(bool rotateBulletToDirection = false);

    /// <summary>
    /// Shoot a bullet, defining the bullet's rotation speed and rotation acceleration.
    /// </summary>
    /// <param name="rotation"></param>
    /// <param name="rotationAcceleration"></param>
    public abstract void Shoot(float rotation, float rotationAcceleration = 0f);

    protected Bullet InitBullet(Vector2 direction, bool rotateBulletToDirection = false) {
        var newBullet = FetchOrCreateBullet();

        HandleBulletObject(newBullet);

        var bullet = newBullet.GetComponent<Bullet>();

        bullet.Initalize(this, bulletSpeed * direction, bulletAcceleration, ShotBulletType, rotateBulletToDirection);

        // If there are listeners to add.
        if (onBulletDestroy != null) {
            // Add them.
            bullet.OnBulletDestroyedEvent += OnBulletDestroyedEvent;
        }
        

        return bullet;
    }

    protected Bullet InitBullet(Vector2 direction, float rotation, float rotationAcceleration = 0f) {
        var newBullet = FetchOrCreateBullet();

        HandleBulletObject(newBullet);

        var bullet = newBullet.GetComponent<Bullet>();

        bullet.Initalize(this, bulletSpeed * direction, bulletAcceleration, ShotBulletType, rotation, rotationAcceleration);

        // If there are listeners to add.
        if (onBulletDestroy != null) {
            // Add them.
            bullet.OnBulletDestroyedEvent += OnBulletDestroyedEvent;
        }


        return bullet;
    }

    /// <summary>
    /// Set the bullet sprite (if default sprite is given), rotation and position.
    /// </summary>
    /// <param name="bulletObj"></param>
    private void HandleBulletObject(GameObject bulletObj) {
        // If a bullet default sprite is given.
        if (BulletDefaultSprites != null) {
            // Set the bullet sprite.
            bulletObj.GetComponent<SpriteRenderer>().sprite = BulletDefaultSprites;
        }

        // Rotate the bullet respectively.
        var tempRotation = new Vector3();
        tempRotation.z += bulletRotationOffset;
        bulletObj.transform.eulerAngles = tempRotation;

        // Set the bullet position to this shooter's position.
        bulletObj.transform.position = transform.position;
    }

    /// <summary>
    /// Fetch a bullet from the object pool. Creates one if none exists in the object pool.
    /// </summary>
    /// <returns>The bullet.</returns>
    private GameObject FetchOrCreateBullet() {
        // Attempt to fetch a bullet from the object pool.
        GameObject newBullet = ObjectPool.Instance.FetchObjectByCondition(IsSameBulletType);

        // If the object pool does not contain a bullet.
        if (newBullet == null) {
            // Instantiate a new bullet.
            newBullet = Instantiate(bulletPrefab);
        } else {
            newBullet.SetActive(true);
        }

        return newBullet;
    }

    private bool IsSameBulletType(GameObject bulletObj) {
        var bullet = bulletObj.GetComponent<Bullet>();

        // If this is not a bullet.
        if (bulletObj.GetComponent<Bullet>() == null) {
            return false;
        }

        return bullet.Type == bulletType;

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
