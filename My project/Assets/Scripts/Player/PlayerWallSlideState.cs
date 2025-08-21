public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!player.IsTouchingWall || player.IsGrounded)
        {
            stateMachine.ChangeState(player.InAirState);
            return;
        }

        if (player.JumpInput)
        {
            stateMachine.ChangeState(player.WallJumpState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.SetVelocity(0, -player.stats.wallSlideSpeed);
    }
}