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
    /// Get the angle to the target.
    /// </summary>
    /// <returns></returns>
    protected float GetStartingAngle() {
        float startAngle;

        // If we are locking on to an enemy.
        if (lockOn) {
            // Get the direction from shooter to player
            Vector2 directionToPlayer = targetTransform.position - transform.position;

            startAngle = ((Vector2)(transform.position) + Vector2.up).GetAngleToPosition(directionToPlayer);
        } else {
            startAngle = shotAngle;
        }

        return startAngle;
    }
}
