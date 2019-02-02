using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Boss : MonoBehaviour, IDisposableObj {

    protected delegate void OnBossDeathDelegate();
    protected delegate void OnSpellEndDelegate();

    protected OnSpellEndDelegate onSpellEnd;
    protected OnBossDeathDelegate onBossDeath;

    [Separator("Base Boss Properties", true)]

    [SerializeField, Tooltip("The max amount of health this boss has.")]
    private int maxHealth;

    [SerializeField, Tooltip("The speed which this boss moves at.")]
    private float speed;

    [Header("Spell Cards")]

    [SerializeField, Tooltip("The spell-cards that this boss possess.")]
    protected SpellCard[] spellCards;

    [Header("Particle Systems")]

    [SerializeField, Tooltip("The respective particle system.")]
    private ParticleSystemHelper transitionParticleSystem;

    [SerializeField]
    private ParticleSystemHelper deathParticleSystem;

    #region Properties

    public bool Invulnerable { get; set; }

    public int Health { get; protected set; }

    public int MaxLife {
        get {
            return spellCards.Length;
        }
    }

    public int Life { get; protected set; }

    public int MaxHealth {
        get {
            return maxHealth;
        }
    }

    public float Speed {
        get {
            return speed;
        }

        set {
            speed = value;
        }
    }

    #endregion

    protected SpellCard currentSpell;

    public int NoOfSpells {
        get {
            return spellCards.Length;
        }
    }

    private void Awake() {
        Health = maxHealth;
        Invulnerable = true;
    }

    private void Start() {
        OnStart();

        deathParticleSystem.onParticleSystemStopped += Dispose;
        transitionParticleSystem.onParticleSystemStopped += TransitionToNextSpell;
    }

    protected abstract void OnStart();

    public void Initalize(int lifeCount) {
        Invulnerable = false;
        InitalizeSpellCards();

        Life = lifeCount;

        PickSpellCard();
        currentSpell.InvokeSpell();
    }

    private void InitalizeSpellCards() {
        foreach (var spellCard in spellCards) {
            spellCard.SpellOwner = this;
            spellCard.Initalize();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // If this enemy is invulnerable, exit.
        if (Invulnerable) { return; }

        // If this enemy touched the player's bullet.
        if (other.CompareTag("PlayerBullet")) {
            var hitBullet = other.gameObject.GetComponent<Bullet>();

            UpdateBossHealth(hitBullet.Damage);

            hitBullet.Dispose();
        }
    }

    protected void UpdateBossHealth(int damage) {

        Health -= damage;

        // If the boss is dead but has a life left.
        if (Health <= 0 && Life > 0) {
            HandleLifeLoss();
        }
        // Boss has no life left.
        else if (Health <= 0 && Life <= 0) {
            HandleBossDeath();
        } else {
            currentSpell.ScaleSpell(damage);
        }
    }

    private void HandleBossDeath() {
        Invulnerable = true;
        currentSpell.EndSpell();
        if (onSpellEnd != null) { onSpellEnd(); }

        if (onBossDeath != null) {
            onBossDeath();
        }

        SpawnManager.Instance.BossFight = false;

        deathParticleSystem.PlayParticleSystem();
    }

    private void HandleLifeLoss() {
        currentSpell.EndSpell();
        PickSpellCard();

        --Life;
        Health = maxHealth;

        transitionParticleSystem.PlayParticleSystem();
    }

    protected void PickSpellCard() {
        do {
            currentSpell = spellCards[Random.Range(0, spellCards.Length)];
        } while (currentSpell.Invoked);
    }

    private void TransitionToNextSpell() {
        if (currentSpell.Invoked) { PickSpellCard(); }

        currentSpell.InvokeSpell();
    }

    public void Dispose() {

        Destroy(gameObject);
    }
}
