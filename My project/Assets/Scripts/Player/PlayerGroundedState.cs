using UnityEngine;

public abstract class PlayerGroundedState : PlayerState
{
    protected PlayerGroundedState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsHealInput && player.StatsManager.CanHeal())
        {
            stateMachine.ChangeState(player.HealState);
            return;
        }
        if (player.IsThrowInput && player.StatsManager.CanThrow() && player.StatsManager.CurrentStamina >= player.stats.throwStaminaCost)
        {
            stateMachine.ChangeState(player.ThrowState);
            return;
        }
        if (player.IsSpecialAttackInput && player.StatsManager.CanUseSpecialAttack() && player.StatsManager.CurrentStamina >= player.stats.specialAttackStaminaCost)
        {
            stateMachine.ChangeState(player.SpecialAttackState);
            return;
        }
        if (player.IsDefendInput)
        {
            stateMachine.ChangeState(player.DefendState);
            return;
        }

        if (player.AttackInput && player.CanAttack() && player.StatsManager.CurrentStamina >= player.stats.attackStaminaCost)
        {
            stateMachine.ChangeState(player.AttackState);
            return;
        }

        if (player.Input.y > 0.5f)
        {
            if (player.IsTouchingLadder)
            {
                stateMachine.ChangeState(player.ClimbState);
                return;
            }
        }
        else if (player.Input.y < -0.5f)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.GroundCheck.position, Vector2.down, 0.5f, player.stats.ladderLayer);
            if (hit.collider != null)
            {
                stateMachine.ChangeState(player.ClimbState);
                return;
            }
            else
            {
                stateMachine.ChangeState(player.CrouchState);
                return;
            }
        }

        if (player.JumpInput && player.StatsManager.CurrentStamina >= player.stats.jumpStaminaCost)
        {
            if (player.Input.y < -0.5f)
            {
                RaycastHit2D hit = Physics2D.Raycast(player.GroundCheck.position, Vector2.down, player.stats.groundCheckDistance, player.stats.groundLayer);
                if (hit.collider != null && hit.collider.GetComponent<PlatformEffector2D>() != null)
                {
                    player.StartCoroutine(player.TemporarilyDisableCollider(hit.collider));
                    return;
                }
            }

            stateMachine.ChangeState(player.JumpState);
        }
        else if (!player.IsGrounded)
        {
            stateMachine.ChangeState(player.InAirState);
        }
        else if (player.DashInput && player.CanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
