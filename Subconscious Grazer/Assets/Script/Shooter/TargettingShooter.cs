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

            startAngle = AngleTo(transform.position, (targetTransform.position));
        } else {
            startAngle = shotAngle;
        }

        return startAngle;
    }

    float AngleBetweenVector2(Vector2 vec1, Vector2 vec2) {
        Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
        float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
        return Vector2.Angle(vec1, vec2) * sign;
    }

    private float AngleTo(Vector2 pos, Vector2 target) {
        Vector2 diference = Vector2.zero;

        diference = target - pos;

        //if (target.y > pos.y) {
        //    diference = target - pos;
        //} else {
        //    diference = pos - target;
        //}

        return Vector2.Angle(Vector2.zero, diference);
    }
}
