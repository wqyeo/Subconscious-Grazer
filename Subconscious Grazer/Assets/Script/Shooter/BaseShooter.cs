using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Linq;

public abstract class BaseShooter : MonoBehaviour {

    public delegate void BulletDelegate(Bullet bullet);

    public BulletDelegate onBulletDestroy;

    public BulletDelegate onBulletCreated;

    [SerializeField, Tooltip("True if this is active on start.")]
    private bool activeOnStart = true;

    [Separator("Base bullet properties", true)]

    [MustBeAssigned, SerializeField, Tooltip("Prefab of the bullet that this shooter shoots out")]
    protected GameObject bulletPrefab;

    [SerializeField, Tooltip("The amount of damage this bullet deals.")]
    private int damage;

    [SerializeField, Tooltip("The type of the bullet this shooter shoots.")]
    private BulletType bulletType;

    [Range(0, 360), SerializeField, Tooltip("How much more to rotate the bullet by. (For the sprite to show correctly.)")]
    private float bulletRotationOffset;

    [SerializeField, Tooltip("The default sprite for the bullets. (Null to just use what was given from prefab or object pool)")]
    private Sprite bulletDefaultSprite;

    [SerializeField, Tooltip("True if the bullets are affected by gravity.")]
    private bool gravityAffected;

    [Separator("Initalized Bullet properties", true)]

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

    [Separator("Batched shooting options.")]

    [SerializeField, Tooltip("True if this shooter shoots in batches.")]
    private bool batchShooting = false;

    [ConditionalField("batchShooting", true), SerializeField, Tooltip("The cooldown between each shot.")]
    private float batchShootCooldown;

    [ConditionalField("batchShooting", true), SerializeField, Tooltip("The number of shots to fire per batch.")]
    private int batchShotCount;

    private HashSet<Bullet> shotBullets;

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

    public int Damage {
        get {
            return damage;
        }

        set {
            damage = value;
        }
    }

    public float OriginalBulletSpeed { get; set; }
    public float OriginalBulletAcceleration { get; set; }

    public bool GravityAffected {
        get {
            return gravityAffected;
        }

        set {
            gravityAffected = value;
        }
    }

    #endregion

    public void Shoot() {
        if (shotBullets == null) { shotBullets = new HashSet<Bullet>(); }

        if (batchShooting) {
            StartCoroutine(ShootBatches());
        } else {
            InvokeShooting();
        }
    }

    protected abstract void InvokeShooting();

    private IEnumerator ShootBatches() {
        for (int i = 0; i < batchShotCount; ++i) {
            InvokeShooting();
            yield return new WaitForSeconds(batchShootCooldown);
        }

        yield return null;
    }

    protected Bullet CreateBulletObject(Vector2 direction) {
        var newBullet = FetchOrCreateBullet();
        var bullet = newBullet.GetComponent<Bullet>();

        newBullet.SetActive(true);

        SetBulletPosition(newBullet);
        SetBulletSprite(newBullet);
        SetBulletRotation(newBullet, direction);

        shotBullets.Add(bullet);

        InitalizeBullet(bullet, direction);

        SetBulletEventListeners(bullet);

        return bullet;
    }

    private void InitalizeBullet(Bullet bullet, Vector2 direction) {
        // If we need to rotate the bullet to the flying direction.
        if (rotateBulletToDirection) {
            // Create a bullet that constantly rotates to the flying direction.
            bullet.Initalize(this, bulletSpeed * direction, bulletAcceleration, damage, ShotBulletType, rotateBulletToDirection, bulletRotationOffset, gravityAffected);
        } else {
            // Create a bullet, giving desired rotation and rotation acceleration.
            bullet.Initalize(this, bulletSpeed * direction, bulletAcceleration, damage, ShotBulletType, rotation, rotationAcceleration, bulletRotationOffset, gravityAffected);
        }
    }

    private void SetBulletEventListeners(Bullet bullet) {
        // If there are listeners to add.
        if (onBulletDestroy != null) {
            // Add them.
            bullet.OnBulletDisposedEvent += OnBulletDestroyedEvent;
        }

        if (onBulletCreated != null) {
            onBulletCreated(bullet);
        }
    }

    private void SetBulletPosition(GameObject bulletObj) {
        bulletObj.transform.position = transform.position;
    }

    private void SetBulletSprite(GameObject bulletObj) {
        // If a bullet default sprite is given.
        if (BulletDefaultSprite != null) {
            // Set the bullet sprite.
            bulletObj.GetComponent<SpriteRenderer>().sprite = BulletDefaultSprite;
        } else {
            // Use the given prefab's sprite.
            bulletObj.GetComponent<SpriteRenderer>().sprite = bulletPrefab.GetComponent<SpriteRenderer>().sprite;
        }
    }

    private void SetBulletRotation(GameObject bulletObj, Vector2 direction) {
        // Set back to the default rotation place before rotating.
        bulletObj.transform.rotation = Quaternion.identity;
        // Rotate based off the offset
        bulletObj.transform.Rotate(new Vector3(0, 0, bulletRotationOffset));

        // If we initally need to rotate this bullet to the shooting direction.
        if (initalRotateToDirection) {
            // Set a rotation where it looks at the new position from the current position.
            Quaternion rotation = Quaternion.LookRotation(direction, transform.TransformDirection(Vector3.up + new Vector3(0, 0, -bulletRotationOffset)));
            // Rotate respectively.
            bulletObj.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        }
    }

    /// <summary>
    /// Fetch a bullet from the object pool. Creates one if none exists in the object pool.
    /// </summary>
    /// <returns>The bullet.</returns>
    private GameObject FetchOrCreateBullet() {
        // Attempt to fetch a bullet from the object pool.
        GameObject newBullet = ObjPoolManager.Instance.BulletPool.FetchObjByType(bulletType);

        // If the object pool does not contain a bullet.
        if (newBullet == null) {
            // Instantiate a new bullet.
            newBullet = Instantiate(bulletPrefab);
            // Add the bullet to the object pool
            ObjPoolManager.Instance.BulletPool.AddToObjectPool(bulletType, newBullet);
        }

        return newBullet;
    }

    private void OnBulletDestroyedEvent(object sender, EventArgs e) {
        // If the delegate still exists.
        if (onBulletDestroy != null) {
            // Invoke delegate.
            onBulletDestroy(sender as Bullet);
        }
    }

    /// <summary>
    /// Determine the direction of the bullet's travel based on the given shooting angle.
    /// </summary>
    /// <param name="shootingAngle"></param>
    /// <returns></returns>
    protected Vector2 DetermineBulletMoveDirection(float shootingAngle) {
        // Determine the direction of the bullet travel on the x and y axis.
        float bulletDirectionX = transform.position.x + Mathf.Sin((shootingAngle * Mathf.PI) / 180);
        float bulletDirectionY = transform.position.y + Mathf.Cos((shootingAngle * Mathf.PI) / 180);

        // Determines the direction this bullet should be moving.
        Vector2 bulletDirection = new Vector2(bulletDirectionX, bulletDirectionY);
        return (bulletDirection - (Vector2)transform.position).normalized;
    }

    public void AddOnBulletDestroyedListener(BulletDelegate listener) {
        if (onBulletDestroy == null) {
            onBulletDestroy = listener;
        } else {
            onBulletDestroy += listener;
        }
    }

    public void RemoveBullet(Bullet bullet) {
        shotBullets.Remove(bullet);
    }

    public void ClearAllOnBulletDestroyedListener() {
        onBulletDestroy = null;
    }

    /// <summary>
    /// Invoke an action onto all active bullets that are shot by this shooter.
    /// </summary>
    /// <param name="invokeAction"></param>
    public void InvokeOnAllShotBullets(Action<Bullet> invokeAction) {
        if (shotBullets == null) { shotBullets = new HashSet<Bullet>(); }

        foreach (var bullet in shotBullets.ToArray()) {
            invokeAction(bullet);
        }
    }
}
