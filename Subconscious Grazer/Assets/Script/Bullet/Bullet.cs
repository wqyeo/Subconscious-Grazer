using UnityEngine;
using System;

[DisallowMultipleComponent]
public class Bullet : MonoBehaviour, IDisposableObj {

    public event EventHandler OnObjectDisposedEvent;

    [Separator("Base Bullet Properties", true)]

    [SerializeField, Tooltip("True if this bullet is affected by gravity.")]
    private bool gravityAffected = false;

    [SerializeField, Tooltip("True if this bullet has an lifespan")]
    private bool hasLifeSpan = false;

    [ConditionalField("hasLifeSpan", true), SerializeField, Tooltip("The lifespan of this bullet")]
    private float bulletLifeSpan;

    private float lifetime;

    /// <summary>
    /// The type of this bullet.
    /// </summary>
    public BulletType Type { get; private set; }

    /// <summary>
    /// The speed and direction at which this bullet is travelling at.
    /// </summary>
    public Vector2 Velocity { get; set; }

    /// <summary>
    /// Speed acceleration of this bullet
    /// </summary>
    public float AccelerationSpeed { get; set; }

    public float RotationalOffset { get; set; }

    /// <summary>
    /// True if we want the bullet to rotate towards the direction it is travelling to. (Overrides constant rotation)
    /// </summary>
    public bool RotateBulletToDirection { get; set; }

    /// <summary>
    /// Constantly rotates the bullet at this speed.
    /// </summary>
    public float RotationSpeed { get; set; }

    /// <summary>
    /// The acceleration to apply to the bullet's constant rotation.
    /// </summary>
    public float RotationAccelerationSpeed { get; set; }

    /// <summary>
    /// The amount of damage this bullet deals.
    /// </summary>
    public int Damage { get; set; }

    public bool Grazed { get; set; }

    public bool GravityAffected {
        get {
            return gravityAffected;
        }

        set {
            gravityAffected = value;
        }
    }

    private BaseShooter parentShooter;

    protected virtual void OnUpdate() { }

    public void Update() {
        UpdateBulletAcceleration(Time.deltaTime);

        UpdateBulletPositionAndRotation(Time.deltaTime);

        UpdateBulletLifeSpan(Time.deltaTime);

        OnUpdate();
    }

    private void UpdateBulletAcceleration(float deltaTime) {
        // Change speed based on acceleration.
        Velocity += (Velocity.normalized * (AccelerationSpeed * deltaTime));

        // If this bullet is affected by gravity.
        if (gravityAffected) {
            Velocity += ((9.81f * deltaTime) * Vector2.down);
        }
    }

    private void UpdateBulletPositionAndRotation(float deltaTime) {
        Vector3 newPosition = (transform.position) + ((Vector3)(Velocity * deltaTime));

        UpdateRotation(newPosition, deltaTime);

        transform.position = (newPosition);
    }

    private void UpdateBulletLifeSpan(float deltaTime) {
        if (hasLifeSpan) {
            lifetime += deltaTime;

            if (lifetime >= bulletLifeSpan) {
                Dispose();
            }
        }
    }

    /// <summary>
    /// Handle the rotation of this bullet.
    /// </summary>
    /// <param name="newPos">Where the bullet would be at.</param>
    private void UpdateRotation(Vector3 newPos, float deltaTime) {
        // If we need to rotate the bullet to the direction it is travelling at.
        if (RotateBulletToDirection) {
            RotateBulletToNewDirection(newPos);
        } 
        // If we need to constantly rotate this bullet.
        else if (RotationSpeed != 0) {
            transform.Rotate(0, 0, RotationSpeed * deltaTime);

            RotationSpeed += (AccelerationSpeed * Time.deltaTime);
        }
    }

    protected void RotateBulletToNewDirection(Vector3 newDirection) {
        // Set a rotation where it looks at the new direction from the current position.
        Quaternion rotation = Quaternion.LookRotation(newDirection.normalized, transform.TransformDirection(Vector3.up + new Vector3(0, 0, -RotationalOffset)));
        // Rotate respectively.
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
    }

    #region Initalize_Overloads

    protected virtual void OnInitalize() { }

    // Rotates bullet to the direction it is travelling at.
    public void Initalize(BaseShooter shooter, Vector2 velocity, float accelerationSpeed, int damage, BulletType bulletType = BulletType.Undefined, bool rotateBulletToDirection = true, float rotationalOffset = 0f, bool gravityAffected = false) {
        Velocity = velocity;
        AccelerationSpeed = accelerationSpeed;
        Type = bulletType;
        RotateBulletToDirection = rotateBulletToDirection;
        RotationSpeed = 0;
        RotationAccelerationSpeed = 0f;
        Damage = damage;
        parentShooter = shooter;
        RotationalOffset = rotationalOffset;
        GravityAffected = gravityAffected;

        bulletLifeSpan = 0;
        Grazed = false;

        OnInitalize();
    }

    // Constantly rotates the bullet at the given speed.
    public void Initalize(BaseShooter shooter, Vector2 velocity, float accelerationSpeed, int damage, BulletType bulletType = BulletType.Undefined, float rotationSpeed = 0f, float rotationAcceleration = 0f, float rotationalOffset = 0f, bool gravityAffected = false) {
        Velocity = velocity;
        AccelerationSpeed = accelerationSpeed;
        Type = bulletType;
        RotateBulletToDirection = false;
        RotationSpeed = rotationSpeed;
        RotationAccelerationSpeed = rotationAcceleration;
        Damage = damage;
        parentShooter = shooter;
        RotationalOffset = rotationalOffset;
        GravityAffected = gravityAffected;

        bulletLifeSpan = 0;
        Grazed = false;

        OnInitalize();
    }

    #endregion

    public void DetachFromShooter() {
        if (parentShooter != null) {
            parentShooter.RemoveBullet(this);
            parentShooter = null;
        }
    }

    private void PoolBackBullet() {
        // Empty eventlistener.
        OnObjectDisposedEvent = null;
        // Set itself to not active
        gameObject.SetActive(false);
    }

    public void Dispose() {

        if (OnObjectDisposedEvent != null) {
            OnObjectDisposedEvent.Invoke(this, null);
        }

        DetachFromShooter();

        PoolBackBullet();
    }
}
