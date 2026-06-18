using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private Coroutine dashCoroutine;

    public PlayerDashState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        player.lastDashTime = Time.time;
        player.IsInvincible = true;
        dashCoroutine = player.StartCoroutine(Dash());
        SetEnemyCollision(false);

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundType.PlayerDash);
    }

    public override void Exit()
    {
        base.Exit();
        player.IsInvincible = false;
        if (dashCoroutine != null)
            player.StopCoroutine(dashCoroutine);
        player.ResetGravity();
        SetEnemyCollision(true);
    }

    // 대시 중 적 레이어와의 물리 충돌을 켜거나 끔
    private void SetEnemyCollision(bool enabled)
    {
        int playerLayer = player.gameObject.layer;
        int enemyMask = player.stats.enemyLayer.value;
        for (int i = 0; i < 32; i++)
        {
            if ((enemyMask & (1 << i)) != 0)
                Physics2D.IgnoreLayerCollision(playerLayer, i, !enabled);
        }
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
