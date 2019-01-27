using UnityEngine;

[System.Serializable]
public class SpellOption {

    [Header("Base Spell Options")]

    [Tooltip("The shooter(s) this spell has.")]
    public BaseShooter[] shooters;

    [Tooltip("The fire-rate of this spell.")]
    public float fireRate;

    [Tooltip("The delay before activating this spell.")]
    public float activationDelay;

    [Header("Scaling Options")]

    [Tooltip("True to scale this spell as the boss's health drops.")]
    public bool scaleByHealth;

    public float OriginalFireRate { get; set; }

    [ConditionalField("scaleByHealth", true), Tooltip("The firerate to scale towards as the boss's health drops.")]
    public float fireRateScale;

    [ConditionalField("scaleByHealth", true), Tooltip("The speed of the bullet to scale towards as the boss's health drops.")]
    public float bulletSpeedScale;

    [ConditionalField("scaleByHealth", true), Tooltip("The acceleration of the bullet to towards as the boss's health drops.")]
    public float bulletAccelerationScale;
}
