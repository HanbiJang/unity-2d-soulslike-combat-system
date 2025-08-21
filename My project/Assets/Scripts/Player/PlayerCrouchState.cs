public class PlayerCrouchState : PlayerGroundedState
{
    public PlayerCrouchState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter(); player.SetVelocity(0, 0); player.SetCollider(player.stats.crouchColliderSize, player.stats.crouchColliderOffset);
    }

    public override void Exit()
    {
        base.Exit();
        player.ResetCollider();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (stateMachine.CurrentState == this)
        {
            if (player.Input.y >= -0.5f)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
