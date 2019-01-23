public abstract class AI {

    public abstract AIType TypeOfAI { get; }

    /// <summary>
    /// The enemy this AI is currently controlling.
    /// </summary>
    public Enemy ControllingEnemy { get; set; }

    protected IShooting shooterEnemy;

    protected float shootTimer;

    public AI(Enemy enemyToControl) {
        ControllingEnemy = enemyToControl;
        shooterEnemy = ControllingEnemy as IShooting;
        shootTimer = 0f;
    }

    /// <summary>
    /// Should be called per frame to update the AI.
    /// </summary>
    /// <param name="time">Delta time</param>
    public abstract void UpdateAI(float time);

    /// <summary>
    /// Should be called before this AI gets swapped out to the next state.
    /// </summary>
    /// <param name="newAIState">The new AI state</param>
    public abstract void SwitchState(AI newAIState);

    /// <summary>
    /// Call this to handle shooting. (If the shooter for the enemy exists.)
    /// </summary>
    /// <param name="time">Time in deltatime of the update.</param>
    protected void HandleShooting(float time) {
        // If the controlled enemy can do shooting.
        if (shooterEnemy != null) {

            shootTimer += time;

            // If the timer is up for shooting.
            if (shootTimer >= shooterEnemy.ShootCooldown) {
                // Shoot.
                shooterEnemy.Shoot();
                // Reset timer
                shootTimer = 0f;
            }
        }
    }
}
