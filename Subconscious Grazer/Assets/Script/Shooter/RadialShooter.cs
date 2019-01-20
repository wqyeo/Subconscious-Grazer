using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialShooter : TargettingShooter {

    [Separator("Radial shooter properties", true)]

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

    public override void Shoot() {
        float angleStep = 360f / bulletCount;
        float angle = GetStartingAngle();

        for (int i = 0; i <= bulletCount - 1; i++) {
            Vector2 bulletMoveDirection = DetermineBulletMoveDirection(angle.GetNormalizedAngle());

            InitBullet(bulletMoveDirection);

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
