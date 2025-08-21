using UnityEngine;

public class PlayerClimbState : PlayerState
{
    private float entryTime; public PlayerClimbState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();

        entryTime = Time.time;
        int layerValue = player.stats.climbingPlayerLayer.value;
        if (layerValue != 0)
        {
            int layerIndex = (int)Mathf.Log(layerValue, 2);
            player.SetLayer(layerIndex);
        }
        player.SetGravity(0f);
        player.SetVelocity(0f, 0f);
    }

    public override void Exit()
    {
        base.Exit();

        int layerValue = player.stats.playerLayer.value;
        if (layerValue != 0)
        {
            int layerIndex = (int)Mathf.Log(layerValue, 2);
            player.SetLayer(layerIndex);
        }

        player.ResetGravity();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time < entryTime + 0.1f)
        {
            return;
        }

        if (!player.IsTouchingLadder)
        {
            if (player.Input.y > 0)
            {
                player.SetVelocity(0, 0);
            }
            stateMachine.ChangeState(player.InAirState);
            return;
        }
        if (player.IsGrounded && player.Input.y < 0.1f)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }

        if (player.JumpInput)
        {
            stateMachine.ChangeState(player.JumpState);
            return;
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocity(0, player.Input.y * player.stats.climbSpeed);
    }
}