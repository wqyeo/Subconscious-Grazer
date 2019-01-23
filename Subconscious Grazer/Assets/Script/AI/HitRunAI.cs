using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRunAI : AI {

    private const AIType typeOfAI = AIType.HitRunAI;

    public override AIType TypeOfAI {
        get {
            return typeOfAI;
        }
    }

    /// <summary>
    /// The linear direction of where this enemy is travelling towards.
    /// </summary>
    public Vector2 MoveDirection { get; private set; }

    private float moveTimer;
    private float lingerTimer;

    private bool isMoving, isRunning, isLingering;
    private bool canShoot;

    // Progress for lerping for the flee.
    private float progress;

    public HitRunAI(Enemy controlledEnemy, Direction moveDirection) : base(controlledEnemy) {
        moveTimer = lingerTimer = 0f;
        progress = 0f;

        isMoving = true;
        isRunning = isLingering = false;

        canShoot = !ControllingEnemy.ShootAfterMoving;

        MoveDirection = moveDirection.ToVector2();
        ControllingEnemy.SetMoveDirection(MoveDirection);
    } 

    public override void SwitchState(AI newAIState) {
        
    }

    public override void UpdateAI(float time) {

        HandleMomvent(time);

        // If this AI can shoot
        if (canShoot) {
            HandleShooting(time);
        }
    }

    private void HandleMomvent(float time) {
        // If this AI is currently moving it it's destination
        if (isMoving) {
            HandleMoving(time);
        }
        // If this AI is lingering around.
        else if (isLingering) {
            // Increment timer.
            HandleLingering(time);
        }
        // If this AI is running
        else {
            HandleRunning(time);
        }
    }
    private void SetMoveDirection(Vector2 moveDirection) {
        MoveDirection = moveDirection;
    }

    #region Moving_Lingering_Running

    private void HandleRunning(float time) {
        // If the enemy just started running. (Start moving slowly.)
        if (progress <= 1) {
            // Determine the velocity this enemy should reach.
            Vector2 finalVelocity = -MoveDirection * ControllingEnemy.Speed;
            ControllingEnemy.Velocity = Vector2.Lerp(Vector2.zero, finalVelocity, progress);

            // Increment progress.
            progress += time * 1.25f;
        } else {
            ControllingEnemy.SetMoveDirection(-MoveDirection);
        }
    }

    private void HandleMoving(float time) {
        // Increment timer.
        moveTimer += time;

        // Move it to it's destination.
        ControllingEnemy.SetMoveDirection(MoveDirection);

        // If the duration for moving is reached
        if (moveTimer >= ControllingEnemy.MoveDuration) {

            // If the enemy just started stopping. (Stop moving slowly.)
            if (progress <= 1) {
                // Determine the velocity when the enemy started slowing down.
                Vector2 startVel = MoveDirection * ControllingEnemy.Speed;
                ControllingEnemy.Velocity = Vector2.Lerp(startVel, Vector2.zero, progress);

                // Increment progress.
                progress += time * 1.25f;
            } else {
                progress = 0f;

                // This enemy is now lingering.
                isMoving = false;
                isLingering = true;
                // Ensures that this enemy can now shoot
                canShoot = true;
                // Ensures this enemy stays
                ControllingEnemy.SetMoveDirection(Vector2.zero);
            }
        }
    }

    private void HandleLingering(float time) {
        // If we just need to keep lingering.
        if (ControllingEnemy.LingerDuration < 0) {
            // Exit
            return;
        }

        // Increment timer.
        lingerTimer += time;

        // If the duration for lingering is reached
        if (lingerTimer >= ControllingEnemy.LingerDuration) {
            // This enemy is now running.
            isLingering = false;
            isRunning = true;
        }
    }

    #endregion
}
