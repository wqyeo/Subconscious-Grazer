using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For shooters that requires a target/angle.
public abstract class TargettingShooter : BaseShooter
{
    [Separator("Shooter Targetting properties", true)]

    [SerializeField, Tooltip("True if this shooter locks on to an enemy")]
    private bool lockOn;

    [ConditionalField("lockOn", false), SerializeField, Tooltip("Where this shot is angled towards."), Range(0f, 360f)]
    private float shootingAngle;
    
    [MustBeAssigned, ConditionalField("lockOn", true), SerializeField, Tooltip("The target this shooter is locking on to. (Play if null.)")]
    private Transform targetTransform;

    [Header("Angle Rotating Options")]

    [SerializeField, ConditionalField("lockOn", false), Tooltip("The rotational speed applied to the shooting angle.")]
    private float shootAngleRotationSpeed;

    #region Properties
    public Transform TargetTransform {
        get {
            return targetTransform;
        }

        set {
            targetTransform = value;
        }
    }

    public float ShotAngle {
        get {
            return shootingAngle;
        }

        set {
            shootingAngle = value;
        }
    }

    public bool LockOn {
        get {
            return lockOn;
        }

        set {
            lockOn = value;
        }
    }

    public float ShootAngleRotationSpeed {
        get {
            return shootAngleRotationSpeed;
        }

        set {
            shootAngleRotationSpeed = value;
        }
    }
    #endregion

    private void Update() {
        RotateShootAngleIfNotLockedOn();
    }

    private void RotateShootAngleIfNotLockedOn() {
        if (!lockOn) {
            shootingAngle += (shootAngleRotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Get the direction to the desired target to shoot at.
    /// </summary>
    /// <returns>The direction to the desired target</returns>
    protected Vector2 GetShootDirection() {
        // If we are locking on to an enemy.
        if (lockOn) {
            TargetPlayerIfTargetIsNull();

            return (targetTransform.position - transform.position).normalized;

        } else {
            return DetermineBulletMoveDirection(shootingAngle);
        }
    }

    private void TargetPlayerIfTargetIsNull() {
        if (targetTransform == null) {
            targetTransform = Player.Instance.transform;
        }
    }
}
