using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Boss : MonoBehaviour {
    [Separator("Base Boss Properties", true)]

    [SerializeField, Tooltip("The max amount of health this boss has.")]
    private int maxHealth;

    [SerializeField, Tooltip("The speed which this boss moves at.")]
    private float speed;

    [SerializeField, Tooltip("The spell-cards that this boss possess.")]
    private SpellCard[] spellCards;

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

    private void Awake() {
        Health = maxHealth;
    }

    private void Start() {
        Initalize(0);
    }

    public void Initalize(int lifeCount) {
        foreach (var spellCard in spellCards) {
            spellCard.Initalize();
        }

        Life = lifeCount;

        PickSpellCard();
        currentSpell.InvokeSpell();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // If this enemy is invulnerable, exit.
        if (Invulnerable) { return; }

        // If this enemy touched the player's bullet.
        if (other.CompareTag("PlayerBullet")) {
            var hitBullet = other.gameObject.GetComponent<Bullet>();

            DamageBoss(hitBullet.Damage);

            hitBullet.Dispose();
        }
    }

    protected void DamageBoss(int damage) {

        Health -= damage;

        // If the boss is dead but has a life left.
        if (Health <= 0 && Life > 0) {
            currentSpell.EndSpell();
            PickSpellCard();

            --Life;
            Health = maxHealth;

            currentSpell.InvokeSpell();
        }
        // Boss has no life left.
        else if (Health <= 0 && Life <= 0) {

            currentSpell.EndSpell();

            Destroy(gameObject);

        } else {
            currentSpell.ScaleSpell(damage);
        }
    }

    protected void PickSpellCard() {
        do {
            currentSpell = spellCards[Random.Range(0, spellCards.Length - 1)];
        } while (currentSpell.Invoked);
    }
}
