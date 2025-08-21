public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(PlayerController player, string stateName) : base(player, stateName) { }
    public override void Enter()
    {
        base.Enter();
        float forceX = player.stats.wallJumpForce.x * (player.IsFacingRight ? -1 : 1);
        float forceY = player.stats.wallJumpForce.y;
        player.SetVelocity(forceX, forceY);
        player.CheckAndFlip(forceX); stateMachine.ChangeState(player.InAirState);
    }
}