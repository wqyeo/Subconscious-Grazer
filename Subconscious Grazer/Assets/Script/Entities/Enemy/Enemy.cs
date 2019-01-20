using UnityEngine;
using System;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D)), DisallowMultipleComponent]
public abstract class Enemy : MonoBehaviour {
    [Separator("Base enemy properties", true)]

    [SerializeField, Tooltip("The move speed of this enemy.")]
    protected float speed;

    [SerializeField, Tooltip("The hitpoints of this enemy.")]
    protected int health;

    protected Rigidbody2D enemyRB;

    protected BaseShooter[] shooters;

    #region Properties

    public int Health {
        get { return health; }
    }

    #endregion

    private void Start() {
        enemyRB = GetComponent<Rigidbody2D>();
        shooters = GetComponentsInChildren<BaseShooter>();
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

    private void OnTriggerEnter2D(Collider2D other) {
        // If this enemy touched the player's bullet.
        if (other.CompareTag("PlayerBullet")) {
            other.gameObject.GetComponent<Bullet>().Dispose();
        }
    }
}
