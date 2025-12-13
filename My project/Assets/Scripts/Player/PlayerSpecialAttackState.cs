using UnityEngine;
public class PlayerSpecialAttackState : PlayerState
{
    private float attackStartTime;
    public PlayerSpecialAttackState(PlayerController player, string stateName) : base(player, stateName) { }
    public override void Enter()
    {
        base.Enter();
        player.StatsManager.TryUseStamina(player.stats.specialAttackStaminaCost);
        attackStartTime = Time.time;
        player.StatsManager.UseSpecialAttackCharge();
        player.SetVelocity(0, 0);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= attackStartTime + player.stats.specialAttackData.attackDuration)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        player.PerformAttack(player.IsGrounded, player.stats.specialAttackData);
    }
}
