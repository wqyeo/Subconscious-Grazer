using UnityEngine;

public class SpreadShooter : BaseShooter {

    [Header("Spread shooter properties")]

    [SerializeField, Tooltip("The amount of bullet this shooter shoots out at once.")]
    private int bulletCount;

    [SerializeField, Tooltip("Where this shot is angled towards."), Range(0f, 360f)]
    private float shotAngle;

    [SerializeField, Tooltip("How wide the shot will spread out.")]
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
            Debug.LogWarning(gameObject.name + " : ArcShooter.cs :: pelletShot must be a positive value!");
        }
    }

    public override void Shoot() {

        float offSet = shotWideness / 2;
        // The angle to rotate after each shot.
        float angleStep = (shotWideness / (bulletCount + 1));
        float angle = angleStep - offSet;

        // For each pellet we have to shoot
        for (int i = 0; i < bulletCount; ++i) {
            // Determine the direction of the bullet travel on the x and y axis.
            float bulletDirectionX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float bulletDirectionY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            // Determines the direction this bullet should be moving.
            Vector2 bulletDirection = new Vector2(bulletDirectionX, bulletDirectionY);
            Vector2 bulletMoveDirection = (bulletDirection - (Vector2)transform.position).normalized;

            //// Get where to shoot at.
            //Vector2 directionToShootAt = ((Vector2)transform.position).Rotate(shotAngle);
            //// Angle correctly to create an arc.
            //directionToShootAt = directionToShootAt.Rotate(angle - offSet);
            // Create the bullet.
            InitBullet(bulletMoveDirection);

            angle += angleStep;
        }
    }

    public void SwitchAngle(float newAngle) {
        shotAngle = newAngle;
    }
}
