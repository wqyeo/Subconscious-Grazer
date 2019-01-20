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
}
