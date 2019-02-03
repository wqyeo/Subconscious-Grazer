using UnityEngine;

public class Tensei : Boss {

    [Separator("Tensei Boss Properties", true)]

    [SerializeField, Tooltip("Shooter for the star vortex")]
    private TargettingShooter starVortexShooter;

    [SerializeField, Tooltip("Fire rate for the star vortex shooter")]
    private float starVortexFireRate;

    [MinMaxRange(-100f, 100f), SerializeField, Tooltip("Min and max rotation speed for the star vortex shooter.")]
    private RangedFloat minMaxRotationSpeed;

    [SerializeField, Tooltip("How frequent it will change it's rotation speed.")]
    private float rotationSpeedChangeFrequency;

    private float rotatingFromSpeed, rotatingToSpeed;

    // Timer before we change the rotating speed again.
    private float rotationChangeTimer;

    private float fireRateTimer;

    protected override void OnStart() {
        rotatingFromSpeed = 0f;
        rotatingToSpeed = starVortexShooter.ShootAngleRotationSpeed;

        onSpellEnd += delegate {
            if (currentSpell.SpellCardName != SpellCardName.Star_Vortex) {
                ShooterActiveBulletsToBonusPoints(starVortexShooter);
            }
        };
    }

    private void Update() {
        if (currentSpell != null) {
            if (!currentSpell.Invoking) { return; }

            if (currentSpell.SpellCardName == SpellCardName.Star_Vortex) {
                UpdateStarVortexSpell(Time.deltaTime);
            }
        }
    }

    private void UpdateStarVortexSpell(float deltaTime) {
        UpdateStarVortexShooting(deltaTime);
        UpdateStarVortexRotation(deltaTime);
    }

    // Updates shooting of the spiral shooters.
    private void UpdateStarVortexShooting(float deltaTime) {
        fireRateTimer += deltaTime;

        if (fireRateTimer >= starVortexFireRate) {
            starVortexShooter.Shoot();
            fireRateTimer = 0f;
        }
    }

    // Update rotating of the spiral shooters.
    private void UpdateStarVortexRotation(float deltaTime) {
        rotationChangeTimer += deltaTime;

        if (rotationChangeTimer >= rotationSpeedChangeFrequency) {
            ChangeRotatingFromAndToSpeed();
            rotationChangeTimer = 0f;
        }

        UpdateRotationSpeed(deltaTime);
    }

    private void ChangeRotatingFromAndToSpeed() {
        rotatingFromSpeed = rotatingToSpeed;
        // Generate another rotation speed for the shooter.
        rotatingToSpeed = Random.Range(minMaxRotationSpeed.Min, minMaxRotationSpeed.Max);
    }

    private void UpdateRotationSpeed(float deltaTime) {
        float t = rotationChangeTimer / rotationSpeedChangeFrequency;

        // Set the rotation speed of the shooter based on the time before we change the rotation speed again.
        float currentRotationSpeed = Mathf.Lerp(rotatingFromSpeed, rotatingToSpeed, t);

        starVortexShooter.ShootAngleRotationSpeed = currentRotationSpeed;
    }
}
