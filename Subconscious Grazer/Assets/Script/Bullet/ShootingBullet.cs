using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseShooter))]
public class ShootingBullet : Bullet {

    [Separator("Shooting Bullet Properties", true)]
    [SerializeField, Tooltip("The fire-rate of the shooters.")]
    private float fireRate;

    private float fireRateTimer;

    private BaseShooter[] shooters;

    #region Properties

    public float FireRate {
        get {
            return fireRate;
        }

        set {
            fireRate = value;
        }
    }

    #endregion

    private void Start() {
        shooters = GetComponents<BaseShooter>();
    }

    protected override void OnUpdate() {
        UpdateShooting(Time.deltaTime);
    }

    private void UpdateShooting(float time) {
        fireRateTimer += time;

        if (fireRateTimer >= fireRate) {
            Shoot();
            fireRateTimer = 0;
        }
    }

    private void Shoot() {
        foreach (var shooter in shooters) {
            if (shooter.IsActive) { shooter.Shoot(); } 
        }
    }

    protected override void OnInitalize() {
        fireRateTimer = 0f;
    }
}
