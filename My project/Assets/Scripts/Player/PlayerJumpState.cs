public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        player.StatsManager.TryUseStamina(player.stats.jumpStaminaCost);

        float jumpForce = player.stats.jumpForce;

        // 직전 상태가 Dash였다면 배로 점프
        if (player.StateMachine.PreviousState == player.RunState)
        {
            jumpForce *= player.stats.dashJumpMultiplier;
        }

        player.SetVelocity(player.Rb.velocity.x* jumpForce, jumpForce);

        // 점프 소리 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.PlayerJump);
        }
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
