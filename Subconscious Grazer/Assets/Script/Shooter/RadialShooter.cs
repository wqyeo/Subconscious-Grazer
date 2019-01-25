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

    protected override void OnShootInvoked() {
        // Find the initial direction to shoot for the first bullet.
        Vector2 initialDirection = FindShootDirection();
        // How much to rotate by after each shot.
        float angleStep = 360f / bulletCount;
        // Current facing angle.
        float angle = 0f;

        // For each bullet to shoot.
        for (int i = 0; i <= bulletCount - 1; i++) {
            // Rotate the initial direction by the current facing angle.
            Vector2 bulletMoveDirection = initialDirection.Rotate(angle.GetNormalizedAngle());
            // Shoot.
            InitBullet(bulletMoveDirection);
            // Rotate respectively.
            angle += angleStep;
        }
    }
}
