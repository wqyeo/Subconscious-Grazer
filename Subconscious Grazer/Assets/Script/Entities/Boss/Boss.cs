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

    [SerializeField, Tooltip("Where this boss should be positioned when not moving around.")]
    private Vector2 defaultPos;

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

    private IEnumerator bossMoveCoroutine;

    /// <summary>
    /// Where the boss last was before moving.
    /// </summary>
    private Vector2 oldPosition;

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
        bossMoveCoroutine = MoveBossByPattern();
        Invulnerable = false;
        InitalizeSpellCards();

        Life = lifeCount;

        // Pick a spellcard and invoke it.
        PickSpellCard();
        currentSpell.InvokeSpell();
        // Start moving the boss
        StartCoroutine(bossMoveCoroutine);
        Debug.Log("Inital move boss");
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
        // Stop moving the boss.
        StopCoroutine(bossMoveCoroutine);

        AudioManager.Instance.PlayAudioClipIfExists(AudioType.BossDeath);
        Invulnerable = true;
        currentSpell.EndSpell();

        HandleItemSpawning();

        // Invoke delegates if needed.
        if (onBossDeath != null) {
            onBossDeath();
        }

        if (onSpellEnd != null) {
            onSpellEnd();
        }

        SpawnManager.Instance.BossFight = false;

        // Reward for defeating boss.
        GameManager.Instance.AddPoints(2000);
        // Play the death particles.
        deathParticleSystem.PlayParticleSystem();
    }

    private void HandleLifeLoss() {
        // Stop moving the boss.
        StopCoroutine(bossMoveCoroutine);

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

    private void TransitionToNextSpell() {
        Invulnerable = false;
        if (currentSpell.Invoked) { PickSpellCard(); }

        currentSpell.InvokeSpell();

        bossMoveCoroutine = MoveBossByPattern();
        StartCoroutine(bossMoveCoroutine);
    }

    protected void PickSpellCard() {
        do {
            currentSpell = spellCards[Random.Range(0, spellCards.Length)];
        } while (currentSpell.Invoked);
    }

    private IEnumerator RunAwayBossThenDispose() {
        float from = transform.position.y;
        float to = 10f;
        float progress = 0f;

        // Progressively move the boss away.
        while (progress <= 1) {
            MoveBossFromToByInYaxisByProgress(from, to, progress);

            progress += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Dispose();
    }

    private void MoveBossFromToByInYaxisByProgress(float from, float to, float progress) {
        /// TODO: Code Refactor!!! (Don't Repeat Yourself, mix with 'LerpVector2'!!)
        var temp = transform.position;
        temp.y = Mathf.Lerp(from, to, progress);
        transform.position = temp;
    }


    private IEnumerator MoveBossByPattern() {
        // There is no spell card pattern yet.
        if (currentSpell == null) {
            yield break;
        }

        oldPosition = transform.position;

        float progress = 0f;

        // If this boss does not need to move.
        if (!currentSpell.MoveBoss) {
            // progressively move the boss to it's default position.
            while (progress < 1f) {
                transform.position = LerpVector2(oldPosition, defaultPos, progress);
                progress += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield break;
        } else {

            Vector2 newPosToMoveTo = PickRandomSpotToMoveFromCurrentSpellCard();
            // how much distance we initally needed to travel.
            float initialDistToTravel = DistanceFromOldPositionToGivenPos(newPosToMoveTo);

            // Constantly move the boss around, until an action invokes to stop this coroutine.
            while (true) {

                // Get the direction we have to travel to.
                var direction = (newPosToMoveTo - (Vector2)transform.position ).normalized;

                // Move the boss by it's speed in the direction.
                transform.position += (Vector3) (direction * (speed * Time.deltaTime));

                // Progress: current distance we traveled, divided by the distance we have to travel.
                progress = DistanceFromOldPositionToGivenPos(transform.position) / initialDistToTravel;

                // If we are approximately at our destination already.
                if (progress >= 0.975f) {
                    // The boss's current position is it's old spot now.
                    oldPosition = transform.position;
                    // Pick a new spot to move to.
                    newPosToMoveTo = PickRandomSpotToMoveFromCurrentSpellCard();
                    // Re-calculate the distance we have to travel now.
                    initialDistToTravel = DistanceFromOldPositionToGivenPos(newPosToMoveTo);
                    // Reset progress.
                    progress = 0f;

                    // Wait for a delay before moving again
                    yield return new WaitForSeconds(1.5f);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        yield return null;
    }

    private float DistanceFromOldPositionToGivenPos(Vector2 position) {
        return Vector2.Distance(oldPosition, position);
    }

    private Vector2 PickRandomSpotToMoveFromCurrentSpellCard() {
        return currentSpell.BossPositions[Random.Range(0, currentSpell.BossPositions.Length)];
    }

    /// <summary>
    /// Lerp a vector2 between from and to, by t.
    /// </summary>
    private Vector2 LerpVector2(Vector2 from, Vector2 to, float t) {
        Vector2 result = new Vector2();
        result.x = Mathf.Lerp(from.x, to.x, t);
        result.y = Mathf.Lerp(from.y, to.y, t);
        return result;
    }

    public void Dispose() {
        Destroy(gameObject);
    }
}
