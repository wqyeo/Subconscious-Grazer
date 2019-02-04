using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, DisallowMultipleComponent]
public class SpellCard : MonoBehaviour {
    [Separator("Spell Card Properties", true)]

    [SearchableEnum, SerializeField, Tooltip("The name of this spell card")]
    private SpellCardName spellCardName;

    [SerializeField, Tooltip("The spell options for this spellcard")]
    private SpellOption[] spellOptions;

    [Range(15, 90), SerializeField, Tooltip("How long does this spellcard lasts for")]
    private int spellDuration;

    public bool Invoked { get; private set; }
    public bool Invoking { get; private set; }

    public Boss SpellOwner { get; set; }

    public SpellCardName SpellCardName {
        get {
            return spellCardName;
        }
    }

    public void Initalize() {
        Invoked = Invoking = false;

        foreach (var spellOption in spellOptions) {
            spellOption.OriginalFireRate = spellOption.fireRate;

            foreach (var shooter in spellOption.shooters) {
                shooter.OriginalBulletAcceleration = shooter.BulletAcceleration;
                shooter.OriginalBulletSpeed = shooter.BulletSpeed;
            }
        }
    }

    public void InvokeSpell() {
        SetSpellState(true);
        Invoked = Invoking = true;

        foreach (var spellOption in spellOptions) {
            StartCoroutine(HandleSpellOption(spellOption));
        }

        GameManager.Instance.WarnSpellCard(spellCardName);
        GameManager.Instance.CountDownSpellCard(spellDuration, SpellOwner.HandleSpellCardTimeOut);
    }

    public void EndSpell() {
        Invoking = false;

        GenerateBonusPointsOnActiveBullets();

        SetSpellState(false);

        GameManager.Instance.HideSpellCardWarningAndStopCountDown();
    }

    private void GenerateBonusPointsOnActiveBullets() {
        // Convert all active bullets to bonus points
        foreach (var spellOption in spellOptions) {
            foreach (var shooter in spellOption.shooters) {
                shooter.ConvertAllActiveBulletsToBonusPoints();
            }
        }
    }

    public void ScaleSpell(int healthLoss = 1) {
        foreach (var spellOption in spellOptions) {
            // If we need to scale this spell option by health.
            if (spellOption.scaleByHealth) {

                ScaleSpellOptionByMissingBossHealth(spellOption);
            }
        }
    }

    private void ScaleSpellOptionByMissingBossHealth(SpellOption spellOption) {
        // Scale the fire rates based on how much health the boss is missing.
        spellOption.fireRate = LerpValueByMissingBossHealth(spellOption.OriginalFireRate, spellOption.OriginalFireRate + spellOption.fireRateScale);

        // Scale the shooters.
        foreach (var shooter in spellOption.shooters) {
            shooter.BulletSpeed = LerpValueByMissingBossHealth(shooter.OriginalBulletSpeed, shooter.OriginalBulletSpeed + spellOption.bulletSpeedScale);
            shooter.BulletAcceleration = LerpValueByMissingBossHealth(shooter.OriginalBulletAcceleration, shooter.OriginalBulletAcceleration + spellOption.bulletAccelerationScale);
        }
    }

    private float LerpValueByMissingBossHealth(float a, float b) {
        float t = (float)(SpellOwner.MaxHealth - SpellOwner.Health) / SpellOwner.MaxHealth;

        return Mathf.Lerp(a, b, t);
    }

    private IEnumerator HandleSpellOption(SpellOption spellOption) {
        // Wait for activiation.
        yield return new WaitForSeconds(spellOption.activationDelay);

        // While this spell card is invoking.
        do {
            // Fire all the shooters
            foreach (var shooter in spellOption.shooters) {
                shooter.Shoot();
            }

            yield return new WaitForSeconds(spellOption.fireRate);
        } while (Invoking);

        yield return null;
    }

    private void SetSpellState(bool isActive) {
        // Set the shooter active states.
        foreach (var spellOption in spellOptions) {
            foreach (var shooter in spellOption.shooters) {
                shooter.IsActive = isActive;
            }
        }

        gameObject.SetActive(isActive);
    }
}