using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearShooter : TargettingShooter {

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

    public override void Shoot() {
        float startAngle = GetStartingAngle();

        Vector2 bulletMoveDirection = DetermineBulletMoveDirection(startAngle);

        InitBullet(bulletMoveDirection);
    }
}
