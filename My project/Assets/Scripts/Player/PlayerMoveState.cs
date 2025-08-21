public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.IsRunInput && player.StatsManager.CurrentStamina > 0)
        {
            stateMachine.ChangeState(player.RunState);
            return;
        }

        if (player.Input.x == 0)
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