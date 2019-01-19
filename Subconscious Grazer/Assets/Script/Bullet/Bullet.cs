using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Bullet : MonoBehaviour {

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

    private BaseShooter parentShooter;

    public void Update() {

        // Change speed based on acceleration.
        Velocity += (Velocity.normalized * (AccelerationSpeed * Time.deltaTime));

        Vector2 newPosition = ((Vector2) transform.position) + (Velocity * Time.deltaTime);

        // Set new position
        transform.position = (newPosition);
    }

    public void Initalize(BaseShooter shooter, Vector2 velocity, float accelerationSpeed, BulletType bulletType = BulletType.Undefined) {
        parentShooter = shooter;
        Velocity = velocity;
        AccelerationSpeed = accelerationSpeed;
        Type = bulletType;
    }
}
