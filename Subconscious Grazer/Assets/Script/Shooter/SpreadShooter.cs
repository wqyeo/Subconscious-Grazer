using UnityEngine;

public class SpreadShooter : TargettingShooter {

    [Separator("Spread shooter properties", true)]

    [SerializeField, Tooltip("The amount of bullet this shooter shoots out at once.")]
    private int bulletCount;

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

    public float ShotAngle {
        get {
            return shotAngle;
        }

        set {
            shotAngle = value;
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
    #endregion

    private void OnValidate() {
        // If the number of pellets to shoot is negative or zero.
        if (bulletCount < 0) {
            // Make it positive and warn.
            bulletCount = 1;
            Debug.LogWarning(gameObject.name + " : ArcShooter.cs :: bulletCount must be a positive value!");
        }
    }

    public override void Shoot() {

        // Get the initial direction to shoot at
        Vector2 initalDirection = FindShootDirection();

        // Offset for the spread. (So that the first shot wont start at the targetted angle, and continue clockwise.)
        float offSet = shotWideness / 2;

        // The angle to rotate after each shot.
        float angleStep = (shotWideness / (bulletCount + 1));
        float angle = (angleStep - offSet);

        // For each pellet we have to shoot
        for (int i = 0; i < bulletCount; ++i) {
            // Find out where the bullet have to move to from the current shooting angle.
            Vector2 bulletMoveDirection = initalDirection.Rotate(angle);
            // Initalize the bullet.
            InitBullet(bulletMoveDirection);

            angle += angleStep;
        }
    }
}
