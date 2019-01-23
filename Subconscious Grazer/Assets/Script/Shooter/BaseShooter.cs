using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseShooter : MonoBehaviour {

    public delegate void OnBulletDestroyed(Bullet destroyedBullet);

    public OnBulletDestroyed onBulletDestroy;

    [SerializeField, Tooltip("True if this is active on start.")]
    private bool activeOnStart = true;

    [Separator("Base bullet properties", true)]

    [MustBeAssigned, SerializeField, Tooltip("Prefab of the bullet that this shooter shoots out")]
    protected GameObject bulletPrefab;

    [SearchableEnum,SerializeField, Tooltip("The type of the bullet this shooter shoots.")]
    private BulletType bulletType;

    [Range(0, 360), SerializeField, Tooltip("How much more to rotate the bullet by. (For the sprite to show correctly.)")]
    private float bulletRotationOffset;

    [SerializeField, Tooltip("The default sprite for the bullets. (Null to just use what was given from prefab or object pool)")]
    private Sprite bulletDefaultSprite;

    [Separator("Initalized Bullet properties", true)]

    [SerializeField, Layer, Tooltip("The layer of the bullet shot out.")]
    private int bulletLayer;

    [TagSelector, SerializeField, Tooltip("The tag of the bullet shot out.")]
    private string bulletTag;

    [SerializeField, Tooltip("The default speed of all the bullets shot out")]
    protected float bulletSpeed;

    [SerializeField, Tooltip("The default acceleration of all the bullets shot out")]
    protected float bulletAcceleration;

    [SerializeField, Tooltip("True if we need to initally rotate the bullet to it's flying direction.")]
    private bool initalRotateToDirection;

    [SerializeField, Tooltip("True to constantly rotate the bullet to the direction it is flying to.")]
    private bool rotateBulletToDirection;

    [SerializeField, ConditionalField("rotateBulletToDirection", false), Tooltip("The constant rotation speed to apply to the bullet.")]
    private float rotation;

    [SerializeField, ConditionalField("rotateBulletToDirection", false), Tooltip("Rotation acceleration value for the bullet.")]
    private float rotationAcceleration;

    #region Property

    public bool IsActive {
        get {
            return activeOnStart;
        }
        set {
            activeOnStart = value;
        }
    }

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

    public float BulletRotationOffset {
        get {
            return bulletRotationOffset;
        }

        set {
            bulletRotationOffset = value;
        }
    }

    public float Rotation {
        get {
            return rotation;
        }

        set {
            rotation = value;
        }
    }

    public float RotationAcceleration {
        get {
            return rotationAcceleration;
        }

        set {
            rotationAcceleration = value;
        }
    }

    public bool RotateBulletToDirection {
        get {
            return rotateBulletToDirection;
        }

        set {
            rotateBulletToDirection = value;
        }
    }

    public bool InitalRotateToDirection {
        get {
            return initalRotateToDirection;
        }

        set {
            initalRotateToDirection = value;
        }
    }

    public Sprite BulletDefaultSprite {
        get {
            return bulletDefaultSprite;
        }

        set {
            bulletDefaultSprite = value;
        }
    }

    #endregion

    /// <summary>
    /// Shoot a bullet.
    /// </summary>
    public abstract void Shoot();

    protected Bullet InitBullet(Vector2 direction) {
        var newBullet = FetchOrCreateBullet();

        HandleBulletObject(newBullet, direction, initalRotateToDirection);

        var bullet = newBullet.GetComponent<Bullet>();

        if (rotateBulletToDirection) {
            bullet.Initalize(bulletSpeed * direction, bulletAcceleration, ShotBulletType, rotateBulletToDirection);
        } else {
            bullet.Initalize(bulletSpeed * direction, bulletAcceleration, ShotBulletType, rotation, rotationAcceleration);
        }

        // If there are listeners to add.
        if (onBulletDestroy != null) {
            // Add them.
            bullet.OnBulletDisposedEvent += OnBulletDestroyedEvent;
        }

        return bullet;
    }

    /// <summary>
    /// Set the bullet sprite (if default sprite is given), rotation and position.
    /// </summary>
    /// <param name="bulletObj"></param>
    private void HandleBulletObject(GameObject bulletObj, Vector2 direction, bool initalRotateToDirection = false) {
        bulletObj.tag = bulletTag;
        bulletObj.layer = bulletLayer;

        // If a bullet default sprite is given.
        if (BulletDefaultSprite != null) {
            // Set the bullet sprite.
            bulletObj.GetComponent<SpriteRenderer>().sprite = BulletDefaultSprite;
        }

        // Rotate the bullet to the offset.
        var tempRotation = new Vector3();
        tempRotation.z += bulletRotationOffset;
        bulletObj.transform.eulerAngles = tempRotation;

        if (initalRotateToDirection) {
            // Set a rotation where it looks at the new position from the current position.
            Quaternion rotation = Quaternion.LookRotation(((Vector3)direction + transform.position) - transform.position, transform.TransformDirection(Vector3.up));
            // Rotate respectively.
            bulletObj.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        }

        // Set the bullet position to this shooter's position.
        bulletObj.transform.position = transform.position;
    }

    /// <summary>
    /// Fetch a bullet from the object pool. Creates one if none exists in the object pool.
    /// </summary>
    /// <returns>The bullet.</returns>
    private GameObject FetchOrCreateBullet() {
        // Attempt to fetch a bullet from the object pool.
        GameObject newBullet = ObjectPool.Instance.FetchObjectByComponent<Bullet>();

        // If the object pool does not contain a bullet.
        if (newBullet == null) {
            // Instantiate a new bullet.
            newBullet = Instantiate(bulletPrefab);
            // Add the bullet to the object pool
            ObjectPool.Instance.AddToPool(newBullet, false);
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

    protected Vector2 DetermineBulletMoveDirection(float shotAngle) {
        // Determine the direction of the bullet travel on the x and y axis.
        float bulletDirectionX = transform.position.x + Mathf.Sin((shotAngle * Mathf.PI) / 180);
        float bulletDirectionY = transform.position.y + Mathf.Cos((shotAngle * Mathf.PI) / 180);

        // Determines the direction this bullet should be moving.
        Vector2 bulletDirection = new Vector2(bulletDirectionX, bulletDirectionY);
        return (bulletDirection - (Vector2)transform.position).normalized;
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
