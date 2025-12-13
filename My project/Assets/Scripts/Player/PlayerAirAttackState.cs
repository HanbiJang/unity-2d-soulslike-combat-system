using UnityEngine;

public class PlayerAirAttackState : PlayerState
{
    private float attackStartTime;
    private AttackData airAttackData;

    public PlayerAirAttackState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();

        player.StatsManager.TryUseStamina(player.stats.airAttackStaminaCost);
        attackStartTime = Time.time;
        player.lastAttackTime = Time.time; airAttackData = player.stats.airAttackData;

        float newXVelocity = player.Rb.velocity.x * player.stats.airAttackMoveSpeedMultiplier;
        player.SetVelocity(newXVelocity, player.Rb.velocity.y);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= attackStartTime + airAttackData.attackDuration)
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        player.PerformAttack(false, airAttackData);
    }
}
