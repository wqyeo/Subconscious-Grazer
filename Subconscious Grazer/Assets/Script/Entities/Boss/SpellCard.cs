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
    }

    public void EndSpell() {
        Invoking = false;

        GenerateBonusPoints();

        SetSpellState(false);
    }

    private void GenerateBonusPoints() {
        foreach (var spellOption in spellOptions) {
            foreach (var shooter in spellOption.shooters) {
                shooter.InvokeOnAllShotBullets(CreateBonusPointOnBullet);
            }
        }
    }

    private void CreateBonusPointOnBullet(Bullet bullet) {
        ItemManager.Instance.CreateCollectableAtPos(bullet.transform.position, ItemType.BonusPoint);
        bullet.Dispose();
    }

    public void ScaleSpell(int healthLoss = 1) {
        foreach (var spellOption in spellOptions) {
            // If we need to scale this spell option by health.
            if (spellOption.scaleByHealth) {

                float t = (float)(SpellOwner.MaxHealth - SpellOwner.Health) / SpellOwner.MaxHealth;

                // Scale the fire rates.
                spellOption.fireRate = Mathf.Lerp(spellOption.OriginalFireRate, spellOption.OriginalFireRate + spellOption.fireRateScale, t);

                // Scale the shooters
                foreach (var shooter in spellOption.shooters) {
                    shooter.BulletSpeed = Mathf.Lerp(shooter.OriginalBulletSpeed, shooter.OriginalBulletSpeed + spellOption.bulletSpeedScale, t);
                    shooter.BulletAcceleration = Mathf.Lerp(shooter.OriginalBulletAcceleration, shooter.OriginalBulletAcceleration + spellOption.bulletAccelerationScale, t);
                }
            }
        }
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