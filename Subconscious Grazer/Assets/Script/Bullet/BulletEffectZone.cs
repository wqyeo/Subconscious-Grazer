using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Collider2D))]
public class BulletEffectZone : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D other) {
        var bullet = other.GetComponent<Bullet>();

        // If this is a bullet.
        if (bullet != null) {
            // If this bullet can be triggered.
            if (bullet is ITriggerable) {
                (bullet as ITriggerable).InvokeEnter();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        var bullet = other.GetComponent<Bullet>();

        // If this is a bullet.
        if (bullet != null) {
            // If this bullet can be triggered.
            if (bullet is ITriggerable) {
                (bullet as ITriggerable).InvokeExit();
            }
        }
    }
}
