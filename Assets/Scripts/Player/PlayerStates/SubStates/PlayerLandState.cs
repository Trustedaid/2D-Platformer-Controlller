using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLandState : PlayerGroundedState
{
    public PlayerLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isExitingState)
        {
            if (xInput != 0)
            {
                //Debug.Log("deneme1");
                stateMachine.ChangeState(player.MoveState);
            }
            else if (isAnimationFinished)
            {
               // Debug.Log("land animasyonu bitti");
                stateMachine.ChangeState(player.IdleState);
            }

        }
    }
}
