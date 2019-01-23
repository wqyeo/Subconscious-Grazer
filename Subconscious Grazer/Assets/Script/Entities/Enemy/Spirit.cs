using UnityEngine;

public class Spirit : Enemy, ITeleport {

    [Separator("Spirit properties", true)]

    [SerializeField, Tooltip("The teleport distance of when teleporting.")]
    private float teleDistance;

    [SerializeField, Tooltip("The duration between each teleport")]
    private float teleDuration;

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
        transform.Translate(direction * teleDistance);
    }
}
