using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Coroutine dashCoroutine;

    public PlayerDashState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        player.lastDashTime = Time.time; player.IsInvincible = true; dashCoroutine = player.StartCoroutine(Dash());
    }
    public override void Exit()
    {
        base.Exit();
        player.IsInvincible = false; if (dashCoroutine != null)
        {
            player.StopCoroutine(dashCoroutine);
        }
        player.ResetGravity();
    }

    private IEnumerator Dash()
    {
        player.Rb.gravityScale = 0f;
        float dashDirection = player.IsFacingRight ? 1f : -1f;

        player.SetVelocity(player.stats.dashSpeed * dashDirection, 0f);
        yield return new WaitForSeconds(player.stats.dashTime);

        player.ResetGravity();
        stateMachine.ChangeState(player.InAirState);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        /*        if (player.AttackInput && player.CanAttack())
                {
                    stateMachine.ChangeState(player.DashAttackState);
                }*/
    }

}
