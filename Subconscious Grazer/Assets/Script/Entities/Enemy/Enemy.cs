using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D)), DisallowMultipleComponent]
public abstract class Enemy : MonoBehaviour {
    [Separator("Base enemy properties", true)]

    [SerializeField, Tooltip("The move speed of this enemy.")]
    private float speed;

    [SerializeField, Tooltip("The hitpoints of this enemy.")]
    protected int health;

    [Separator("Enemy AI properties", true)]

    [SearchableEnum, MustBeAssigned, SerializeField, Tooltip("The type of AI assigned to this enemy at start.")]
    private AIType startAIType;

    [SearchableEnum, MustBeAssigned, SerializeField, Tooltip("Where this AI will start moving towards. (For non-seeker based AI)")]
    private Direction startDirection;

    [ConditionalField("startAIType", AIType.HitRunAI), SerializeField, Tooltip("How long this AI move before lingering")]
    private float moveDuration;

    [ConditionalField("startAIType", AIType.HitRunAI), SerializeField, Tooltip("How long this AI will stay in the lingering spot before running away. (Negative for stay after move)")]
    private float lingerDuration;

    [ConditionalField("startAIType", AIType.HitRunAI), SerializeField, Tooltip("True if this AI only starts shooting after moving.")]
    private bool shootAfterMoving;

    protected Rigidbody2D enemyRB;

    protected AI controllingAI;

    protected BaseShooter[] shooters;

    #region Properties

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
        private set {
            health = value;
        }
    }

    public float Speed {
        get {
            return speed;
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

    #endregion

    private void Start() {
        
        enemyRB = GetComponent<Rigidbody2D>();
        shooters = GetComponentsInChildren<BaseShooter>();

        foreach (var shooter in shooters) {
            // If this shooter requires a target
            if (shooter is TargettingShooter) {
                // Target the player.
                (shooter as TargettingShooter).TargetTransform = Player.Instance.transform;
            }
        }

        AssignAI(startAIType);
    }

    private void Update() {
        controllingAI.UpdateAI(Time.deltaTime);
    }

    /// <summary>
    /// Set where the enemy should move towards.
    /// </summary>
    /// <param name="direction">Where to move this enemy to.</param>
    public void SetMoveDirection(Vector2 direction) {
        enemyRB.velocity = direction * speed;
    }

    /// <summary>
    /// Invoke an action on all shooters.
    /// </summary>
    /// <param name="foreachAction"></param>
    public void ForeachShooter(Action<BaseShooter> foreachAction) {
        foreach (var shooter in shooters) {
            foreachAction(shooter);
        }
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
        } else {
            controllingAI = assignedAI;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // If this enemy touched the player's bullet.
        if (other.CompareTag("PlayerBullet")) {
            other.gameObject.GetComponent<Bullet>().Dispose();
        }
    }

    #region overrideable

    protected virtual void OnEnemyDeath() { };

    #endregion
}
