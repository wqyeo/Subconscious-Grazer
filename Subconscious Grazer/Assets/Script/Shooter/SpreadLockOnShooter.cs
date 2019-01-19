using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class SpreadLockOnShooter : BaseShooter {
    [Header("Spread shooter properties")]

    [SerializeField, Tooltip("The amount of bullet this shooter shoots out at once.")]
    private int bulletCount;

    [SerializeField, Tooltip("The target this shooter is locking on to.")]
    private Transform targetTransform;

    [SerializeField, Tooltip("How wide the shot will spread out."), Range(0f, 360f)]
    private float shotWideness;

    #region Property
    public int BulletCount {
        get {
            return bulletCount;
        }
        set {
            bulletCount = value;
        }
    }

    public float ShotWideness {
        get {
            return shotWideness;
        }

        set {
            shotWideness = value;
        }
    }

    public Transform TargetTransform {
        get {
            return targetTransform;
        }

        set {
            targetTransform = value;
        }
    }

    #endregion

    private void OnValidate() {
        // If the number of pellets to shoot is negative or zero.
        if (bulletCount < 0) {
            // Make it positive and warn.
            bulletCount = 1;
            Debug.LogWarning(gameObject.name + " : ArcShooter.cs :: bulletCount must be a positive value!");
        }
    }


    public override void Shoot(bool rotateBulletToDirection = false) {
        float offSet = shotWideness / 2f;
        float angleStep = (shotWideness / (bulletCount + 1));
        float angle = angleStep - offSet;

        // For each pellet we have to shoot
        for (int i = 0; i < bulletCount; ++i) {

            // Get where to shoot at.
            Vector2 directionToShootAt = targetTransform.position - transform.position;
            // Angle correctly to create an arc.
            directionToShootAt = directionToShootAt.Rotate(angle);
            // Create the bullet.
            InitBullet(directionToShootAt.normalized, rotateBulletToDirection);
        }
    }

    public override void Shoot(float rotation, float rotationAcceleration = 0) {
        float offSet = shotWideness / 2f;
        float angleStep = (shotWideness / (bulletCount + 1));
        float angle = angleStep - offSet;

        // For each pellet we have to shoot
        for (int i = 0; i < bulletCount; ++i) {

            // Get where to shoot at.
            Vector2 directionToShootAt = targetTransform.position - transform.position;
            // Angle correctly to create an arc.
            directionToShootAt = directionToShootAt.Rotate(angle);
            // Create the bullet.
            InitBullet(directionToShootAt.normalized, rotation, rotationAcceleration);
        }
    }

    private Vector2 DetermineBulletMoveDirection(float shotAngle) {
        // Determine the direction of the bullet travel on the x and y axis.
        float bulletDirectionX = transform.position.x + Mathf.Sin((shotAngle * Mathf.PI) / 180);
        float bulletDirectionY = transform.position.y + Mathf.Cos((shotAngle * Mathf.PI) / 180);

        // Determines the direction this bullet should be moving.
        Vector2 bulletDirection = new Vector2(bulletDirectionX, bulletDirectionY);
        return (bulletDirection - (Vector2)transform.position).normalized;
    }
}
