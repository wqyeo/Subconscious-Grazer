using UnityEngine;

public class Rakuki : Boss {

    [Separator("Rakuki Boss Properties", true)]

    [Header("Arrow Rain")]
    [SerializeField, Tooltip("Firerate for the arrow rain.")]
    private float arrowRainFireRate;

    [SerializeField, Tooltip("The shooter for the arrow rain.")]
    private BaseShooter arrowRainShooter;

    [Header("Returning Arrow")]
    [SerializeField, Tooltip("Firate for the returning arrow")]
    private float returningArrowFireRate;

    [SerializeField, Tooltip("The shooter for the returning arrow")]
    private BaseShooter returningArrowShooter; 

    private float timer;

    private void Update() {
        if (currentSpell != null) {
            if (!currentSpell.Invoking) { return; }

            if (currentSpell.SpellCardName == SpellCardName.Arrow_Storm) {
                UpdateArrowStormSpell(Time.deltaTime);
            } else if (currentSpell.SpellCardName == SpellCardName.Returning_Arrow) {
                UpdateReturningArrowSpell(Time.deltaTime);
            }
        }
    }

    private void UpdateReturningArrowSpell(float deltaTime) {
        timer += deltaTime;

        if (timer >= returningArrowFireRate) {
            ReturnExisitingReturningArrows();
            returningArrowShooter.Shoot();
            timer = 0f;
        }
    }

    private void ReturnExisitingReturningArrows() {
        AudioManager.Instance.PlayAudioClipIfExists(AudioType.EnemyShoot_Flutter);
        returningArrowShooter.InvokeOnAllShotBullets(SeekBulletToBoss);
    }

    private void SeekBulletToBoss(Bullet bullet) {
        // Make the bullet move towards the boss faster.
        bullet.Velocity = 5f * (transform.position - bullet.gameObject.transform.position).normalized;
        bullet.AccelerationSpeed = 1f;

        // Rotate the bullet to face the boss.
        bullet.RotateBulletToDirection = true;
        // Detach from shooter. (Prevent further calls to this bullet from the shooter.)
        bullet.DetachFromShooter();
    }

    private void UpdateArrowStormSpell(float deltaTime) {
        timer += deltaTime;

        if (timer >= arrowRainFireRate) {
            // Set where to fire the arrow and fire it.
            SetArrowRainPos();
            FireArrowRain();
            timer = 0f;
        }
    }

    private void SetArrowRainPos() {
        float xFirePos = Random.Range(50f, Screen.width + 50f);

        arrowRainShooter.gameObject.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(new Vector2(xFirePos, Screen.height + 10f));
    }

    private void FireArrowRain() {
        arrowRainShooter.Shoot();
    }

    protected override void OnStart() {
        onSpellEnd += delegate {
            if (currentSpell.SpellCardName != SpellCardName.Arrow_Storm) {
                ShooterActiveBulletsToBonusPoints(arrowRainShooter);
            }
        };
    }
}
