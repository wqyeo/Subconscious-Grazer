using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMoveAI : AI {

    private const AIType typeOfAI = AIType.LinearMoveAI;

    public override AIType TypeOfAI {
        get {
            return typeOfAI;
        }
    }

    /// <summary>
    /// The linear direction of where this enemy is travelling towards.
    /// </summary>
    public Vector2 MoveDirection { get; private set; }

    public LinearMoveAI(Enemy controlledEnemy, Direction moveDirection) : base(controlledEnemy) {
        MoveDirection = moveDirection.ToVector2();
        ControllingEnemy.SetMoveDirection(MoveDirection);
    }

    public override void SwitchState(AI newAIState) {

    }

    public override void UpdateAI(float time) {
        HandleShooting(time);
    }

    private void SetMoveDirection(Vector2 moveDirection) {
        MoveDirection = moveDirection;
        ControllingEnemy.SetMoveDirection(MoveDirection);
    }
}
