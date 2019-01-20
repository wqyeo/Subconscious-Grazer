using UnityEngine;

/// <summary>
/// Create an AI that flies by the player
/// </summary>
public class DriveByPlayerAI : AI {

    private Transform playerTransform;

    public DriveByPlayerAI(Enemy enemyToControl) : base(enemyToControl) {
        playerTransform = Player.Instance.transform;
    }

    public override void SwitchState(AI newAIState) {
        
    }

    public override void UpdateAI(float time) {
        
    }
}
