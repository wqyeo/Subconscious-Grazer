using UnityEngine;

[RequireComponent(typeof(BaseShooter))]
public class Fairy : Enemy, IShooting {

    [Separator("Fairy properties", true)]

    [SerializeField, Tooltip("The cooldown before this fairy can shoot again.")]
    private float shootCooldown;

    public float ShootCooldown {
        get {
            return shootCooldown;
        }
    }

    public void Shoot() {
        // Foreach shooter this fairy has.
        foreach (var shooter in shooters) {
            // Shoot if this shooter is active
            if (shooter.IsActive) {
                shooter.Shoot();
            }
        }
    }
}
