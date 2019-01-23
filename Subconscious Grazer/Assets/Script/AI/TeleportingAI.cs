using UnityEngine;

public class TeleportingAI : AI {
    private const AIType typeOfAI = AIType.TeleportingAI;

    public override AIType TypeOfAI {
        get {
            return typeOfAI;
        }
    }

    public Vector2 TeleDirection { get; set; }

    private ITeleport teleportableEnemy;
    private float teleportTimer;

    public TeleportingAI(Enemy controllingEnemy, Direction direction) : base(controllingEnemy) {
        teleportableEnemy = controllingEnemy as ITeleport;
        teleportTimer = 0f;
        TeleDirection = direction.ToVector2();
    }

    public override void SwitchState(AI newAIState) {}

    public override void UpdateAI(float time) {
        HandleTeleporting(time);
        HandleShooting(time);    
    }

    private void HandleTeleporting(float time) {
        // If this enemy can teleport
        if (teleportableEnemy != null) {
            // Increment timer.
            teleportTimer += time;

            // If the enemy can teleport now
            if (teleportTimer >= teleportableEnemy.TeleDuration) {
                // Teleport the enemy.
                teleportableEnemy.Teleport(TeleDirection);
                // Reset timer.
                teleportTimer = 0f;
            }
        }
    }
}
