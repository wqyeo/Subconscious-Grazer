using UnityEngine;
using System;

[DisallowMultipleComponent]
public class Bullet : MonoBehaviour {

    public event EventHandler OnBulletDisposedEvent;

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

    public void Update() {

        // Change speed based on acceleration.
        Velocity += (Velocity.normalized * (AccelerationSpeed * Time.deltaTime));

        Vector3 newPosition = (transform.position) + ((Vector3) (Velocity * Time.deltaTime));

        HandleRotation(newPosition, Time.deltaTime);

        // Set new position
        transform.position = (newPosition);
    }

    /// <summary>
    /// Handle the rotation of this bullet.
    /// </summary>
    /// <param name="newPos">Where the bullet would be at.</param>
    /// <param name="time">Delta time</param>
    private void HandleRotation(Vector3 newPos, float time) {
        // If we need to rotate the bullet to the direction it is travelling at.
        if (RotateBulletToDirection) {
            // Set a rotation where it looks at the new position from the current position.
            Quaternion rotation = Quaternion.LookRotation(newPos - transform.position, transform.TransformDirection(Vector3.up));
            // Rotate respectively.
            transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
        } 
        // If we need to constantly rotate this bullet.
        else if (RotationSpeed != 0) {
            transform.Rotate(0, 0, RotationSpeed * time);

            RotationSpeed += (AccelerationSpeed * Time.deltaTime);
        }
    }

    #region Initalize_Overloads

    // Rotates bullet to the direction it is travelling at.
    public void Initalize(Vector2 velocity, float accelerationSpeed, int damage, BulletType bulletType = BulletType.Undefined, bool rotateBulletToDirection = true) {
        Velocity = velocity;
        AccelerationSpeed = accelerationSpeed;
        Type = bulletType;
        RotateBulletToDirection = rotateBulletToDirection;
        RotationSpeed = 0;
        RotationAccelerationSpeed = 0f;
        Damage = damage;
    }

    // Constantly rotates the bullet at the given speed.
    public void Initalize(Vector2 velocity, float accelerationSpeed, int damage, BulletType bulletType = BulletType.Undefined, float rotationSpeed = 0f, float rotationAcceleration = 0f) {
        Velocity = velocity;
        AccelerationSpeed = accelerationSpeed;
        Type = bulletType;
        RotateBulletToDirection = false;
        RotationSpeed = rotationSpeed;
        RotationAccelerationSpeed = rotationAcceleration;
        Damage = damage;
    }

    #endregion

    public void Dispose(bool destroyBullet = false) {

        if (OnBulletDisposedEvent != null) {
            OnBulletDisposedEvent.Invoke(this, null);
        }

        // If we do not need to destroy this bullet.
        if (!destroyBullet) {
            // Empty eventlistener.
            OnBulletDisposedEvent = null;
            // Set itself to not active
            gameObject.SetActive(false);
        } else {
            Destroy(gameObject);
        }
    }
}
