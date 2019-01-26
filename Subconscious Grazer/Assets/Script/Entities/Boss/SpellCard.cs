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

    public SpellCardName SpellCardName {
        get {
            return spellCardName;
        }
    }

    public void Initalize() {
        Invoked = Invoking = false;
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
        SetSpellState(false);
    }

    public void ScaleSpell(int healthLoss = 1) {
        foreach (var spellOption in spellOptions) {
            // If we need to scale this spell option by health.
            if (spellOption.scaleByHealth) {

                float lossScale = healthLoss / 10f;

                // Scale the fire rates.
                spellOption.fireRate += (lossScale * spellOption.fireRateScale);

                // Scale the shooters
                foreach (var shooter in spellOption.shooters) {
                    shooter.BulletSpeed += (lossScale * spellOption.bulletSpeedScale);
                    shooter.BulletAcceleration += (lossScale * spellOption.bulletAccelerationScale);
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
        gameObject.SetActive(isActive);

        // Set the shooter active states.
        foreach (var spellOption in spellOptions) {
            foreach (var shooter in spellOption.shooters) {
                shooter.IsActive = isActive;
            }
        }
    }
}