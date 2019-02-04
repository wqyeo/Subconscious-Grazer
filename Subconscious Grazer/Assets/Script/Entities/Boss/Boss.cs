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

        deathParticleSystem.onParticleSystemStopped += delegate { StartCoroutine(RunAwayBossThenDispose()); };
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

        // If this enemy touched the player's bullet.
        if (other.CompareTag("PlayerBullet")) {
            var hitBullet = other.gameObject.GetComponent<Bullet>();

            DamageAndUpdateBossHealth(hitBullet.Damage);

            hitBullet.Dispose();
        }
    }

    protected void DamageAndUpdateBossHealth(int damage) {
        // If this boss is invulernable, exit
        if (Invulnerable) { return; }

        GameManager.Instance.SetHealthBarValue((float) Health / maxHealth);
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
        AudioManager.Instance.PlayAudioClipIfExists(AudioType.BossDeath);
        Invulnerable = true;
        currentSpell.EndSpell();
        if (onSpellEnd != null) { onSpellEnd(); }

        HandleItemSpawning();

        if (onBossDeath != null) {
            onBossDeath();
        }

        SpawnManager.Instance.BossFight = false;

        // Reward for defeating boss.
        GameManager.Instance.AddPoints(2000);

        deathParticleSystem.PlayParticleSystem();
    }

    private void HandleLifeLoss() {
        // Boss is invulnerable until it transitions to the next spell.
        Invulnerable = true;

        // The boss has another spell card, fill back up the health bar.
        GameManager.Instance.FillHealthBar();
        Health = maxHealth;
        --Life;

        HandleItemSpawning();
        AudioManager.Instance.PlayAudioClipIfExists(AudioType.BossTransition);
        // End the current spell and pick a new one.
        currentSpell.EndSpell();
        PickSpellCard();

        // Reward for transition to next spell card.
        GameManager.Instance.AddPoints(1000);

        transitionParticleSystem.PlayParticleSystem();
    }

    private void HandleItemSpawning() {
        // Generate a random number of power points and blue points to spawn.
        int powerPointSpawnCount = Random.Range(4, 8);
        int bluePointSpawnCount = Random.Range(8, 12);

        // Spawn those collectable around the enemy.
        while (powerPointSpawnCount > 0) {
            SpawnItemOfTypeAroundEnemy(ItemType.PowerPoint);
            --powerPointSpawnCount;
        }

        while (bluePointSpawnCount > 0) {
            SpawnItemOfTypeAroundEnemy(ItemType.BluePoint);
            --bluePointSpawnCount;
        }
    }

    public void HandleSpellCardTimeOut() {
        // Penalty for not clearing the spell card in time.
        GameManager.Instance.AddPoints(-500);

        // If the boss has a life left.
        if (Life > 0) {
            HandleLifeLoss();
        }
        // Boss has no life left.
        else if (Life <= 0) {
            HandleBossDeath();
        }
    }

    private void SpawnItemOfTypeAroundEnemy(ItemType itemType) {
        ItemManager.Instance.CreateCollectableAtPos((Random.insideUnitSphere * 0.75f) + transform.position, itemType);
    }

    protected void PickSpellCard() {
        do {
            currentSpell = spellCards[Random.Range(0, spellCards.Length)];
        } while (currentSpell.Invoked);
    }

    private void TransitionToNextSpell() {
        Invulnerable = false;
        if (currentSpell.Invoked) { PickSpellCard(); }

        currentSpell.InvokeSpell();
    }

    private IEnumerator RunAwayBossThenDispose() {
        float from = transform.position.y;
        float to = 10f;
        float progress = 0f;

        // Progressively move the boss away.
        while (progress <= 1) {
            MoveBossFromToByProgress(from, to, progress);

            progress += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Dispose();
    }

    private void MoveBossFromToByProgress(float from, float to, float progress) {
        var temp = transform.position;
        temp.y = Mathf.Lerp(from, to, progress);
        transform.position = temp;
    }

    public void Dispose() {
        Destroy(gameObject);
    }
}
