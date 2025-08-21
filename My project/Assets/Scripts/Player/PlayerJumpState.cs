public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(player.Rb.velocity.x, player.stats.jumpForce);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.AttackInput && player.CanAttack())
        {
            stateMachine.ChangeState(player.AirAttackState);
            return;
        }

        if (player.AttackInput && player.CanAttack())
        {
            stateMachine.ChangeState(player.AttackState);
            return;
        }

        /*                if (player.DashInput && player.CanDash())
                {
                    stateMachine.ChangeState(player.DashState);
                    return;
                }*/

        if (player.Rb.velocity.y < 0)
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.CheckAndFlip(player.Input.x);
        player.SetVelocity(player.stats.moveSpeed * player.Input.x, player.Rb.velocity.y);
    }
}