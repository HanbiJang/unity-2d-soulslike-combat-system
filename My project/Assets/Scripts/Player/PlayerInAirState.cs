public class PlayerInAirState : PlayerState
{
    public PlayerInAirState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.IsTouchingWall && player.Input.x != 0)
        {
            stateMachine.ChangeState(player.WallSlideState);
            return;
        }

        if (player.AttackInput && player.CanAttack() && player.StatsManager.CurrentStamina >= player.stats.airAttackStaminaCost)
        {
            stateMachine.ChangeState(player.AirAttackState);
            return;
        }

        if (player.AttackInput && player.CanAttack() && player.StatsManager.CurrentStamina >= player.stats.attackStaminaCost)
        {
            stateMachine.ChangeState(player.AttackState);
            return;
        }

        /*                if (player.DashInput && player.CanDash())
                {
                    stateMachine.ChangeState(player.DashState);
                    return;
                }*/

        if (player.IsTouchingLadder && player.Input.y != 0)
        {
            stateMachine.ChangeState(player.ClimbState);
            return;
        }

        if (player.IsGrounded)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.CheckAndFlip(player.Input.x);
        player.SetVelocity(player.stats.moveSpeed * player.Input.x, player.Rb.velocity.y);
    }
}
