using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D)), DisallowMultipleComponent]
public abstract class Enemy : MonoBehaviour, IDisposableObj {
    [Separator("Base enemy properties", true)]

    [SearchableEnum, SerializeField, Tooltip("The type of this enemy.")]
    private EnemyType typeOfEnemy;

    [SerializeField, Tooltip("The move speed of this enemy.")]
    private float speed;

    [SerializeField, Tooltip("The hitpoints of this enemy.")]
    protected int health;

    [Separator("Enemy AI properties", true)]

    [SearchableEnum, MustBeAssigned, SerializeField, Tooltip("The type of AI assigned to this enemy at start.")]
    private AIType startAIType;

    [SearchableEnum, MustBeAssigned, SerializeField, Tooltip("Where this AI will start moving towards. (For non-seeker based AI)")]
    private Direction startDirection;

    [ConditionalField("startAIType", AIType.HitRunAI), SerializeField, Tooltip("How long this AI moves before lingering")]
    private float moveDuration;

    [ConditionalField("startAIType", AIType.HitRunAI), SerializeField, Tooltip("How long this AI will stay in the lingering spot before running away. (Negative for stay after move)")]
    private float lingerDuration;

    [ConditionalField("startAIType", AIType.HitRunAI), SerializeField, Tooltip("True if this AI only starts shooting after moving.")]
    private bool shootAfterMoving;

    [Separator("Death Properties", true)]

    [SerializeField, Tooltip("True if this enemy explodes on death.")]
    private bool explodeOnDeath;

    [SerializeField, Tooltip("The shooters to invoke when this enemy dies.")]
    protected BaseShooter[] onDeathShooters;

    protected Rigidbody2D enemyRB;

    protected AI controllingAI;

    protected BaseShooter[] shooters;

    protected Animator enemyAnim;

    public bool JustSpawned { get; set; }

    public bool CanAct { get; set; }

    #region Properties
    public bool Invulnerable { get; set; }

    public Vector2 Velocity {
        get {
            return enemyRB.velocity;
        }
        set {
            enemyRB.velocity = value;
        }
    }

    public int Health {
        get {
            return health;
        }
        protected set {
            health = value;
        }
    }

    public float Speed {
        get {
            return speed;
        }
        protected set {
            speed = value;
        }
    }

    /// <summary>
    /// The AI object that is controlling this enemy.
    /// </summary>
    public AI ControllingAI {
        get {
            return controllingAI;
        }
    }

    /// <summary>
    /// Where this enemy is currently moving. (Updated and used by certain AI types.)
    /// </summary>
    public Direction MoveDirection {
        get {
            return startDirection;
        }

        set {
            startDirection = value;
        }
    }

    public float MoveDuration {
        get {
            return moveDuration;
        }

        set {
            moveDuration = value;
        }
    }

    public float LingerDuration {
        get {
            return lingerDuration;
        }

        set {
            lingerDuration = value;
        }
    }

    public bool ShootAfterMoving {
        get {
            return shootAfterMoving;
        }

        set {
            shootAfterMoving = value;
        }
    }

    public bool ExplodeOnDeath {
        get {
            return explodeOnDeath;
        }

        set {
            explodeOnDeath = value;
        }
    }

    public EnemyType TypeOfEnemy {
        get {
            return typeOfEnemy;
        }

        set {
            typeOfEnemy = value;
        }
    }

    #endregion

    private void Start() {
        enemyRB = GetComponent<Rigidbody2D>();
        shooters = GetComponentsInChildren<BaseShooter>();
        enemyAnim = GetComponent<Animator>();

        foreach (var deathShooter in onDeathShooters) {
            deathShooter.IsActive = false;
        }

        CanAct = true;

        AssignAI(startAIType);
    }

    private void Update() {
        if (CanAct) {
            controllingAI.UpdateAI(Time.deltaTime);
        }
    }

    /// <summary>
    /// Set where the enemy should move towards.
    /// </summary>
    /// <param name="direction">Where to move this enemy to.</param>
    public void SetMoveDirection(Vector2 direction) {
        enemyRB.velocity = direction * speed;
    }

    public void AssignAI(AIType typeToAssign) {
        AI assignedAI = null;

        switch (typeToAssign) {
            case AIType.DriveByAI:
                assignedAI = new DriveByPlayerAI(this);
                break;
            case AIType.LinearMoveAI:
                assignedAI = new LinearMoveAI(this, startDirection);
                break;
            case AIType.HitRunAI:
                assignedAI = new HitRunAI(this, startDirection);
                break;
            case AIType.TeleportingAI:
                assignedAI = new TeleportingAI(this, startDirection);
                break;
        }

        if (controllingAI != null) {
            controllingAI.SwitchState(assignedAI);
        }
        controllingAI = assignedAI;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // If this enemy is invulnerable, exit.
        if (Invulnerable) { return; }

        // If this enemy touched the player's bullet.
        if (other.CompareTag("PlayerBullet")) {
            var hitBullet = other.gameObject.GetComponent<Bullet>();

            DamageEnemy(hitBullet.Damage);

            hitBullet.Dispose();
        }
    }

    private void DamageEnemy(int damage) {
        // Reduce health
        Health -= damage;

        // If the enemy is dead
        if (Health <= 0) {
            CanAct = false;
            enemyRB.velocity = Vector2.zero;
            HandleExplodeOnDeath();

            HandleItemSpawning();

            AudioManager.Instance.PlayAudioClipIfExists(AudioType.EnemyDeath);

            // If this enemy has an animator.
            if (enemyAnim != null) {
                // Play it's death animation.
                enemyAnim.Play("DeathClip");
            } else {
                // Set this enemy to inactive.
                gameObject.SetActive(false);
            }
        }
    }

    private void HandleItemSpawning() {
        // Generate a random number of power points and blue points to spawn.
        int powerPointSpawnCount = Random.Range(0, 2);
        int bluePointSpawnCount = Random.Range(1, 4);

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
        ItemManager.Instance.CreateCollectableAtPos((Random.insideUnitSphere * 0.5f) + transform.position, itemType);
    }

    private void HandleExplodeOnDeath() {
        // If this enemy explodes on death
        if (explodeOnDeath) {
            // Set all shooter to inactive.
            ForeachShooter((BaseShooter shooter) => { shooter.IsActive = false; });

            // Fetch all deathshooter
            foreach (var deathShooter in onDeathShooters) {
                // Shoot them.
                deathShooter.Shoot();
            }
        }
    }

    #region Util

    public void InitEnemy(AIType aIType) {
        JustSpawned = true;
        CanAct = true;
        gameObject.SetActive(true);

        if (enemyRB == null) {
            enemyRB = GetComponent<Rigidbody2D>();
        }

        if (shooters == null) {
            shooters = GetComponentsInChildren<BaseShooter>();
        }

        if (enemyAnim == null) {
            enemyAnim = GetComponent<Animator>();
        }

        // If this enemy has an animator.
        if (enemyAnim != null) {
            // Make it play the default animation.
            enemyAnim.Play("Default");
        }

        foreach (var shooter in shooters) {
            // Set the shooters to active
            shooter.IsActive = true;
        }

        foreach (var deathShooter in onDeathShooters) {
            deathShooter.IsActive = false;
        }

        startAIType = aIType;

        AssignAI(aIType);
    }

    public void Dispose() {
        gameObject.SetActive(false);

        // Make sure the object scales back to normal.
        var temp = gameObject.transform.localScale;
        temp.y = 1f;
        temp.x = 1f;
        gameObject.transform.localScale = temp;

        // Make sure the object color turns back to normal.
        var renderTemp = GetComponent<SpriteRenderer>().color;
        renderTemp.a = 1f;
        GetComponent<SpriteRenderer>().color = renderTemp;
    }

    /// <summary>
    /// Invoke an action on all shooters.
    /// </summary>
    /// <param name="foreachAction"></param>
    public void ForeachShooter(System.Action<BaseShooter> foreachAction) {
        foreach (var shooter in shooters) {
            foreachAction(shooter);
        }
    }

    protected abstract void OnCopyDetails(Enemy enemy);

    /// <summary>
    /// Copy the detail of the given enemy over to this enemy.
    /// </summary>
    /// <param name="enemy">The details of the enemy to copy.</param>
    public void CopyDetails(Enemy enemy, bool copyAIType = false) {
        Invulnerable = enemy.Invulnerable;
        Health = enemy.Health;
        Speed = enemy.Speed;
        MoveDirection = enemy.MoveDirection;
        LingerDuration = enemy.LingerDuration;
        ShootAfterMoving = enemy.ShootAfterMoving;
        ExplodeOnDeath = enemy.ExplodeOnDeath;

        OnCopyDetails(enemy);

        if (copyAIType) {
            AssignAI(enemy.controllingAI.TypeOfAI);
        }
    }

    #endregion
}
