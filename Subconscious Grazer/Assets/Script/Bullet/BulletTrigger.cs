using UnityEngine;

[DisallowMultipleComponent]
public class BulletTrigger : Bullet, ITriggerable {

    public delegate void OnBulletTrigger(BulletTrigger bullet);

    public OnBulletTrigger onEnterTriger;

    [SerializeField, Tooltip("True if this bullet trigger can only be invoked once")]
    private bool oneTimeEffect = true;

    [Separator("Bullet on enter trigger effects", true)]
    [SerializeField, Tooltip("True to invoke an effect when a bullet enters a certain trigger region.")]
    private bool invokeOnTriggerEnterEff;

    [ConditionalField("invokeOnTriggerEnterEff", true), SerializeField, Tooltip("Effects when the bullet enters the trigger.")]
    private BulletTriggerEffect onTriggerEnterEff;

    [Separator("Bullet on exit trigger effects", true)]
    [SerializeField, Tooltip("True to invoke an effect when a bullet exists a certain trigger region.")]
    private bool invokeOnTriggerExitEff;

    [ConditionalField("invokeOnTriggerExitEff", true), SerializeField, Tooltip("Effects when bullet exits the trigger.")]
    private BulletTriggerEffect onTriggerExitEff;

    private bool invoked = false;

    public BulletTriggerEffect OnTriggerEnterEff {
        get {
            return onTriggerEnterEff;
        }
    }

    public BulletTriggerEffect OnTriggerExitEff {
        get {
            return onTriggerExitEff;
        }
    }

    protected override void OnInitalize() {
        invoked = false;
    }

    public void InvokeEnter() {

        // If we do not need to invoke, exit
        if (!invokeOnTriggerEnterEff) { return; }

        // If its a one-time effect and the effect has been invoked before, exit
        if (oneTimeEffect && invoked) { return; }

        invoked = true;

        ApplyEffects(onTriggerEnterEff);

        if (onEnterTriger != null) {
            onEnterTriger(this);
        }
    }

    public void InvokeExit() {
        if (!invokeOnTriggerExitEff) { return; }

        ApplyEffects(onTriggerExitEff);
    }

    private void ApplyEffects(BulletTriggerEffect effect) {
        // Change the acceleration if we need to.
        if (effect.applyAcceleration) {
            AccelerationSpeed = effect.triggerAcceleration;
        }

        if (effect.applyRotationalAcceleration) {
            RotationAccelerationSpeed = effect.triggerRotationalAcceleration;
        }

        if (effect.applyRotation) {
            RotationSpeed = effect.triggerRotationSpeed;
        }

        ApplySpeedAndDirectionByEffect(effect);
    }

    private void ApplySpeedAndDirectionByEffect(BulletTriggerEffect effect) {
        // If we need to change both the speed and the direction.
        if (effect.applySpeed && effect.changeDirection) {
            // Apply both at once.
            float speed = effect.triggeredSpeed;
            Vector2 direction = GetNewDirectionForBullet(effect);
            Velocity = speed * direction;

            RotateBulletToNewDirectionIfNeeded(effect, direction);
        } else if (effect.applySpeed) {
            // Apply speed only.
            Velocity = Velocity.normalized * effect.triggeredSpeed;
        } else if (effect.changeDirection) {
            // Change direction only.
            ApplyDirectionalEffects(effect);
        }
    }

    private void ApplyDirectionalEffects(BulletTriggerEffect effect) {
        float speed = (Velocity.magnitude);

        Vector2 direction = GetNewDirectionForBullet(effect);

        Velocity = speed * direction;

        RotateBulletToNewDirectionIfNeeded(effect, direction);
    }

    private void RotateBulletToNewDirectionIfNeeded(BulletTriggerEffect effect, Vector2 direction) {
        // If we need to rotate the bullet to it's flying direction
        if (effect.RotateToNewDirection) {
            RotateBulletToNewDirection(direction);
        }
    }

    private Vector2 GetNewDirectionForBullet(BulletTriggerEffect effect) {

        Vector2 direction;

        // If we need to make the bullet fly towards the player.
        if (effect.DirectionTargetPlayer) {
            var target = Player.Instance.transform;

            // Find out how much this bullet would have to rotate.
            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);

            direction = (target.position - transform.position).normalized;
        } else {
            direction = effect.NewDirection.normalized;
        }

        return direction;
    }
}
