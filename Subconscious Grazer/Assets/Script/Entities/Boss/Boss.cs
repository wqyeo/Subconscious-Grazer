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
        AudioManager.Instance.PlayAudioClipIfExists(AudioType.BossDeath);
        Invulnerable = true;
        currentSpell.EndSpell();
        if (onSpellEnd != null) { onSpellEnd(); }

        HandleItemSpawning();

        if (onBossDeath != null) {
            onBossDeath();
        }

        SpawnManager.Instance.BossFight = false;

        deathParticleSystem.PlayParticleSystem();
    }

    private void HandleLifeLoss() {
        AudioManager.Instance.PlayAudioClipIfExists(AudioType.BossTransition);
        currentSpell.EndSpell();
        PickSpellCard();

        HandleItemSpawning();

        --Life;
        Health = maxHealth;

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

    private void SpawnItemOfTypeAroundEnemy(ItemType itemType) {
        ItemManager.Instance.CreateCollectableAtPos((Random.insideUnitSphere * 0.75f) + transform.position, itemType);
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

    protected void ShooterActiveBulletsToBonusPoints(BaseShooter shooter) {
         shooter.InvokeOnAllShotBullets(CreateBonusPointOnBulletAndDisposeBullet);
    }

    protected void CreateBonusPointOnBulletAndDisposeBullet(Bullet bullet) {
        ItemManager.Instance.CreateCollectableAtPos(bullet.transform.position, ItemType.BonusPoint);
        bullet.Dispose();
    }
}
