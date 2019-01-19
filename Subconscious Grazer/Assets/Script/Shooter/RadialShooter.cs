using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialShooter : BaseShooter {

    [Header("Radial shooter properties")]

    [SerializeField, Tooltip("The amount of bullet this shooter shoots out at once.")]
    private int bulletCount;

    #region Property

    public int BulletCount {
        get {
            return bulletCount;
        }
        set {
            bulletCount = value;
        }
    }

    #endregion

    public override void Shoot(float rotation, float rotationAcceleration = 0) {
        // Find how much to shift the angle of the shot by after each shot.
        float angleStep = 360f / bulletCount;
        // Starting angle of the shot.
        float angle = 0f;

        // For each bullet to shoot.
        for (int i = 0; i <= bulletCount - 1; i++) {
            // Find out which direction the bullet should go based on the shooting angle.
            Vector2 bulletMoveDirection = DetermineBulletMoveDirection(angle);
            // Initalize bullet.
            InitBullet(bulletMoveDirection, rotation, rotationAcceleration);
            // Shift the angle of the shot.
            angle += angleStep;
        }
    }

    public override void Shoot(bool rotateBulletToDirection = false) {
        float angleStep = 360f / bulletCount;
        float angle = 0f;

        for (int i = 0; i <= bulletCount - 1; i++) {
            Vector2 bulletMoveDirection = DetermineBulletMoveDirection(angle);

            InitBullet(bulletMoveDirection, rotateBulletToDirection);

            angle += angleStep;
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
