public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        float minRunStamina = player.stats.staminaDrainRate * player.stats.minRunStartDuration;
        if (player.IsRunInput && player.Input.x != 0 && player.StatsManager.CurrentStamina >= minRunStamina)
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
