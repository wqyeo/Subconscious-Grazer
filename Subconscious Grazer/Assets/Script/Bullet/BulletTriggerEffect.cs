using UnityEngine;

[System.Serializable]
public class BulletTriggerEffect {

    [System.Serializable]
    private class DirectionEff {
        [SerializeField, Tooltip("True to change the direction towards the player")]
        public bool targetPlayer;

        [SerializeField, Tooltip("The direction to set the bullet to when the trigger invokes."), ConditionalField("targetPlayer", false)]
        public Vector2 newDirection;

        [SerializeField, Tooltip("True to rotate the bullet to it's new direction.")]
        public bool rotateToNewDirection;
    }

    #region Accelerations
    [Header("Acceleration")]

    [SerializeField, Tooltip("True to apply an acceleration on the bullet when the trigger invokes.")]
    public bool applyAcceleration;

    [ConditionalField("applyAcceleration", true), SerializeField, Tooltip("The acceleration to apply to the bullet when the trigger invokes.")]
    public float triggerAcceleration;

    [SerializeField, Tooltip("True to apply a rotational acceleration on the bullet when the trigger invokes.")]
    public bool applyRotationalAcceleration;

    [ConditionalField("applyRotationalAcceleration", true), SerializeField, Tooltip("The rotational acceleration to apply to the bullet when the trigger invokes.")]
    public float triggerRotationalAcceleration;
    #endregion

    #region Speed
    [Header("Speed")]

    [SerializeField, Tooltip("True to apply a speed on the bullet when the trigger invokes.")]
    public bool applySpeed;

    [ConditionalField("applySpeed", true), SerializeField, Tooltip("The speed of the bullet when the trigger invokes.")]
    public float triggeredSpeed;

    [SerializeField, Tooltip("True to apply a rotation on the bullet when the trigger invokes.")]
    public bool applyRotation;

    [ConditionalField("applyRotation", true), SerializeField, Tooltip("The rotation speed of the bullet when the trigger invokes.")]
    public float triggerRotationSpeed;
    #endregion

    #region Direction
    [Header("Direction")]

    [SerializeField, Tooltip("True to change the bullet's direction when the trigger invokes.")]
    public bool changeDirection;

    [ConditionalField("changeDirection", true), SerializeField, Tooltip("The direction properties.")]
    private DirectionEff directionEffect;

    #endregion

    public bool DirectionTargetPlayer {
        get {
            return directionEffect.targetPlayer;
        }
    }

    public Vector2 NewDirection {
        get {
            return directionEffect.newDirection;
        }
    }

    public bool RotateToNewDirection {
        get {
            return directionEffect.rotateToNewDirection;
        }
    }
}
