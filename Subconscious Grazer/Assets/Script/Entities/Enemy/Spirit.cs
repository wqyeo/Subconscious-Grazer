using UnityEngine;

public class Spirit : Enemy, ITeleport {

    [Separator("Spirit properties", true)]

    [SerializeField, Tooltip("The teleport distance of when teleporting.")]
    private float teleDistance;

    [SerializeField, Tooltip("The duration between each teleport")]
    private float teleDuration;

    private Vector2 delayedTeleDirection;

    #region Properties

    public float TeleDistance {
        get {
            return teleDistance;
        }

        set {
            teleDistance = value;
        }
    }

    public float TeleDuration {
        get {
            return teleDuration;
        }

        set {
            teleDuration = value;
        }
    }

    #endregion

    public void Teleport(Vector2 direction) {
        // If this spirit has an animator
        if (enemyAnim != null) {
            delayedTeleDirection = direction;
            // Play the teleport animation clip.
            enemyAnim.Play("TeleportClip");
        } else {
            transform.Translate(direction * teleDistance);
        }
    }

    public override void CopyDetails(Enemy enemy) {
        CopyBaseDetails(enemy);

        if (enemy is Spirit) {
            var enemyType = enemy as Spirit;

            TeleDistance = enemyType.TeleDistance;
            TeleDuration = enemyType.TeleDuration;
        }
    }

    public void Teleport() {
        transform.Translate(delayedTeleDirection * teleDistance);
    }
}
