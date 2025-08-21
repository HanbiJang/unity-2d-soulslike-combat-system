public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, player.Rb.velocity.y);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.Input.x != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
}