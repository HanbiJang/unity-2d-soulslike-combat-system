using UnityEngine;

public class PlayerDashAttackState : PlayerState
{
    private float attackStartTime;
    private AttackData dashAttackData;
    private bool isGroundedAttack;

    public PlayerDashAttackState(PlayerController player, string stateName) : base(player, stateName) { }

    public void SetIsGroundedAttack(bool isGrounded)
    {
        this.isGroundedAttack = isGrounded;
    }

    public override void Enter()
    {
        player.StatsManager.TryUseStamina(player.stats.dashAttackStaminaCost);
        attackStartTime = Time.time;
        player.lastAttackTime = Time.time;
        dashAttackData = player.stats.dashAttackData;

        if (player.Anim != null)
        {
            player.Anim.Play(dashAttackData.animationName);
        }

        float moveSpeed = player.stats.dashSpeed * 0.5f;
        player.SetVelocity(moveSpeed * (player.IsFacingRight ? 1 : -1), 0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= attackStartTime + dashAttackData.attackDuration)
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        player.PerformAttack(this.isGroundedAttack, dashAttackData);
    }
}
