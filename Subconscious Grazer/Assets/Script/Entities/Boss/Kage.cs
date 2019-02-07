using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kage : Boss {

    [Separator("Kage Boss Properties", true)]

    [Header("Mirror On The Walls (Kage's Spell Properties)")]

    [SerializeField, Tooltip("The shooters at the left side of the screen.")]
    private BaseShooter leftWallShooter;

    [SerializeField]
    private BaseShooter rightWallShooter;

    [SerializeField, Tooltip("Fire rates for the wall shooters")]
    private float wallShooterFireRate;

    [Header("Danmaku Paranoia (Kage's Spell Properties)")]
    [SerializeField, Tooltip("The shooter for the danmaku paranoia.")]
    private BaseShooter paranoiaShooter;

    [SerializeField, Tooltip("Fire rate for the danmaku paranoia shooter")]
    private float paranoiaShooterFireRate;

    [Header("Illusionary Scare (Kage's Spell Properties)")]
    [SerializeField, Tooltip("The shooter for illusionary scare.")]
    private SpreadShooter illusionaryShooter;

    [SerializeField]
    private BaseShooter scareShooter;

    [SerializeField]
    private float illusionaryShooterFireRate;

    [SerializeField, Tooltip("Delay to invoke the scare afte shooting the illusionaries")]
    private float scareDelay;

    private bool invokeScareDelay;

    private float scareTimer;

    private float fireRateTimer;

    protected override void OnStart() {
        invokeScareDelay = false;
        SetWallShooterPositions();
        SetParanoiaShooterPos();

        onSpellEnd += delegate {
            paranoiaShooter.ConvertAllActiveBulletsToBonusPoints();
            illusionaryShooter.ConvertAllActiveBulletsToBonusPoints();
        };
    }

    private void SetParanoiaShooterPos() {
        // Paranoia shooter goes below the screen.
        Vector2 temp = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f));
        paranoiaShooter.transform.position = (temp - new Vector2(0, 1f));
    }

    private void SetWallShooterPositions() {
        // Set the wall shooter position to be at the left/right side of the screen.
        Vector2 temp = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height / 2f));
        leftWallShooter.transform.position = (temp - new Vector2(1f, 0));
        temp = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height / 2f));
        rightWallShooter.transform.position = (temp + new Vector2(1f, 0));
    }

    private void Update() {
        if (currentSpell != null) {
            if (!currentSpell.Invoking) { return; }

            if (currentSpell.SpellCardName == SpellCardName.Mirror_On_The_Walls) {
                UpdateMirrorOnTheWallsSpell(Time.deltaTime);
            } else if (currentSpell.SpellCardName == SpellCardName.Danmaku_Paranoia) {
                UpdateDanmakuParanoiaSpell(Time.deltaTime);
            } else if (currentSpell.SpellCardName == SpellCardName.Illusionary_Scare) {
                UpdateIllusionaryScareSpell(Time.deltaTime);
            }
        }
    }

    private void UpdateIllusionaryScareSpell(float deltaTime) {
        UpdateIllusionaryShooter();

        fireRateTimer += deltaTime;

        // If we need to invoke the scare.
        if (invokeScareDelay) {
            // Update it.
            UpdateScare();
            scareTimer += deltaTime;
        }
    }

    private void UpdateIllusionaryShooter() {
        // If we can shoot.
        if (fireRateTimer >= illusionaryShooterFireRate) {
            // Shoot.
            illusionaryShooter.Shoot();
            fireRateTimer = 0f;
            // Start invoking the scare.
            invokeScareDelay = true;
        }
    }

    private void UpdateScare() {
        // If we can scare.
        if (scareTimer >= scareDelay) {
            // Start the scare.
            StartCoroutine(InvokeIllusionaryScare());
            scareTimer = 0f;
            // Scare has started, stop invoking.
            invokeScareDelay = false;
        }
    }

    private IEnumerator InvokeIllusionaryScare() {

        Time.timeScale = 0f;

        GameManager.Instance.SetScareFlashPanelAlphaColor(160);

        AudioManager.Instance.PlayAudioClipIfExists(AudioType.EnemyShoot_Flutter);

        // The number of bullets to change at a time before pausing.
        int batchesToChangeAtATime = illusionaryShooter.BulletCount;
        // Keep track of how many bullets we already changed.
        int i = 0;
        foreach (var activeBullet in illusionaryShooter.GetAllActiveShotBullets()) {
            // Move the scare shooter to the current bullet position.
            scareShooter.transform.position = activeBullet.transform.position;
            // Shoot the scare shooter.
            scareShooter.Shoot();

            activeBullet.Dispose();

            ++i;

            // If we already changed enough bullets.
            if (i == batchesToChangeAtATime) {
                // Give it a pause before changing the next batch.
                i = 0;
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        float progress = 0f;

        while (progress < 1f) {
            // Progressively scale back the panel's alpha color.
            GameManager.Instance.SetScareFlashPanelAlphaColor((byte)Mathf.Lerp(160, 0, progress));

            // Progressively scale back time.
            Time.timeScale = Mathf.Lerp(0, 1, progress);

            progress += Time.unscaledDeltaTime * 1.25f;
            yield return new WaitForEndOfFrame();
        }

        // Ensure time and panel is scaled properly at the end.
        Time.timeScale = 1f;
        GameManager.Instance.SetScareFlashPanelAlphaColor(0);

        yield return null;
    }

    private void UpdateDanmakuParanoiaSpell(float deltaTime) {
        fireRateTimer += deltaTime;

        if (fireRateTimer >= paranoiaShooterFireRate) {
            ShootParanoiaShooter();
            fireRateTimer = 0f;
        }
    }

    private void ShootParanoiaShooter() {
        AdjustParanoiaShooterPos();

        paranoiaShooter.Shoot();
    }

    private void AdjustParanoiaShooterPos() {
        var temp = paranoiaShooter.transform.position;
        temp.x = GetRandomHorizontalPointOnScreen();
        paranoiaShooter.transform.position = temp;
    }

    private void UpdateMirrorOnTheWallsSpell(float deltaTime) {
        fireRateTimer += deltaTime;

        if (fireRateTimer >= wallShooterFireRate) {
            ShootWallShooters();
            fireRateTimer = 0f;
        }
    }

    private void ShootWallShooters() {
        AdjustWallShooterPos(leftWallShooter);
        AdjustWallShooterPos(rightWallShooter);

        leftWallShooter.Shoot();
        rightWallShooter.Shoot();
    }

    private void AdjustWallShooterPos(BaseShooter shooter) {
        var temp = shooter.transform.position;
        temp.y = GetRandomVerticlePointOnTopHalfScreen();
        shooter.transform.position = temp;
    }

    /// <summary>
    /// Get a random Y Point on the Top half of the screen.
    /// </summary>
    /// <returns></returns>
    private float GetRandomVerticlePointOnTopHalfScreen() {
        var temp = Random.Range(Screen.height / 2f, Screen.height);

        return Camera.main.ScreenToWorldPoint(new Vector2(0f, temp)).y;
    }

    private float GetRandomHorizontalPointOnScreen() {
        var temp = Random.Range(0f, Screen.width);

        return Camera.main.ScreenToWorldPoint(new Vector2(temp, 0)).x;
    }
}
