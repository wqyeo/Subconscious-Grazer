using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For shooters that requires a target/angle.
public abstract class TargettingShooter : BaseShooter
{
    [Separator("Shooter Targetting properties", true)]

    [SerializeField, Tooltip("True if this shooter locks on to an enemy")]
    protected bool lockOn;

    [ConditionalField("lockOn", false), SerializeField, Tooltip("Where this shot is angled towards."), Range(0f, 360f)]
    protected float shotAngle;

    [MustBeAssigned, ConditionalField("lockOn", true), SerializeField, Tooltip("The target this shooter is locking on to.")]
    protected Transform targetTransform;

    /// <summary>
    /// Get the direction to the desired target to shoot at.
    /// </summary>
    /// <returns>The direction to the desired target</returns>
    protected Vector2 FindShootDirection() {
        // If we are locking on to an enemy.
        if (lockOn) {
            // Find out how much this bullet would have to rotate.
            Quaternion rotation = Quaternion.LookRotation(targetTransform.position - transform.position, Vector3.up);

            return (targetTransform.position - transform.position).normalized;

        } else {
            return DetermineBulletMoveDirection(shotAngle);
        }
    }
}
