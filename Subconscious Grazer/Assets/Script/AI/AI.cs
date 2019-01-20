public abstract class AI {
    /// <summary>
    /// The enemy this AI is currently controlling.
    /// </summary>
    public Enemy ControllingEnemy { get; set; }

    public AI(Enemy enemyToControl) {
        ControllingEnemy = enemyToControl;
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
}
