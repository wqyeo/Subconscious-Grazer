using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearShooter : BaseShooter {

    [Header("Linear shooter properties")]

    [SerializeField, Tooltip("Where this shot is angled towards."), Range(0f, 360f)]
    private float shotAngle;

    #region Property

    public float ShotAngle {
        get {
            return shotAngle;
        }

        set {
            shotAngle = value;
        }
    }

    #endregion

    public override void Shoot(bool rotateBulletToDirection = false) {
        Vector2 bulletMoveDirection = DetermineBulletMoveDirection();

        InitBullet(bulletMoveDirection, rotateBulletToDirection);
    }

    public override void Shoot(float rotation, float rotationAcceleration = 0) {
        Vector2 bulletMoveDirection = DetermineBulletMoveDirection();

        InitBullet(bulletMoveDirection, rotation, rotationAcceleration);
    }

    private Vector2 DetermineBulletMoveDirection() {
        // Determine the direction of the bullet travel on the x and y axis.
        float bulletDirectionX = transform.position.x + Mathf.Sin((shotAngle * Mathf.PI) / 180);
        float bulletDirectionY = transform.position.y + Mathf.Cos((shotAngle * Mathf.PI) / 180);

        // Determines the direction this bullet should be moving.
        Vector2 bulletDirection = new Vector2(bulletDirectionX, bulletDirectionY);
        return (bulletDirection - (Vector2)transform.position).normalized;
    }
}
