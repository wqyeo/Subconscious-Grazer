﻿using UnityEngine;

public class SpreadShooter : BaseShooter {
    [SerializeField, Tooltip("The amount of bullet this shooter shoots out at once.")]
    private int bulletCount;

    [SerializeField, Tooltip("Where this shot is angled towards."), Range(0f, 360f)]
    private float shotAngle;

    [SerializeField, Tooltip("How wide the shot will spread out.")]
    private float shotWideness;

    [SerializeField, Tooltip("The default pellets of bullets to shoot.")]
    private int pelletShot;

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

    public int PelletShot {
        get {
            return pelletShot;
        }

        set {
            pelletShot = value;
        }
    }
    #endregion

    private void OnValidate() {
        // If the number of pellets to shoot is negative or zero.
        if (pelletShot < 0) {
            // Make it positive and warn.
            pelletShot = 1;
            Debug.LogWarning(gameObject.name + " : ArcShooter.cs :: pelletShot must be a positive value!");
        }
    }

    public override void Shoot() {

        float offSet = shotWideness / 2f;

        // For each pellet we have to shoot
        for (int i = 0; i < pelletShot; ++i) {

            // The angle to rotate after each shot.
            float anglePerShot = (shotWideness / (pelletShot + 1)) - offSet;
            // Get where to shoot at.
            Vector2 directionToShootAt = ((Vector2)transform.position).Rotate(shotAngle);
            // Angle correctly to create an arc.
            directionToShootAt = directionToShootAt.Rotate(anglePerShot - offSet);
            // Create the bullet.
            InitBullet(directionToShootAt.normalized);
        }
    }

    public void SwitchAngle(float newAngle) {
        shotAngle = newAngle;
    }
}