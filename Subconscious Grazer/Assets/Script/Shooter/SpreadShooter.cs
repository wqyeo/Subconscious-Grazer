using UnityEngine;

public class SpreadShooter : BaseShooter {

    [Header("Spread shooter properties")]

    [SerializeField, Tooltip("The amount of bullet this shooter shoots out at once.")]
    private int bulletCount;

    [SerializeField, Tooltip("Where this shot is angled towards."), Range(0f, 360f)]
    private float shotAngle;

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

    public override void Shoot(bool rotateBulletToDirection = false) {
        // Offset for the spread. (So that the first shot wont start at the targetted angle, and continue clockwise.)
        float offSet = shotWideness / 2;

        // The angle to rotate after each shot.
        float angleStep = (shotWideness / (bulletCount + 1));
        float angle = shotAngle + (angleStep - offSet);

        // For each pellet we have to shoot
        for (int i = 0; i < bulletCount; ++i) {
            // Find out where the bullet have to move to from the current shooting angle.
            Vector2 bulletMoveDirection = DetermineBulletMoveDirection(angle);
            // Initalize the bullet.
            InitBullet(bulletMoveDirection, rotateBulletToDirection);

            angle += angleStep;
        }
    }

    public override void Shoot(float rotation, float rotationAcceleration = 0) {
        // Offset for the spread. (So that the first shot wont start at the targetted angle, and continue clockwise.)
        float offSet = shotWideness / 2;

        // The angle to rotate after each shot.
        float angleStep = (shotWideness / (bulletCount + 1));
        float angle = shotAngle + (angleStep - offSet);

        // For each pellet we have to shoot
        for (int i = 0; i < bulletCount; ++i) {
            // Find out where the bullet have to move to from the current shooting angle.
            Vector2 bulletMoveDirection = DetermineBulletMoveDirection(angle);
            // Initalize the bullet.
            InitBullet(bulletMoveDirection, rotation, rotationAcceleration);

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
