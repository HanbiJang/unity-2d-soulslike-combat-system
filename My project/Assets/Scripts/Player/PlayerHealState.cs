using UnityEngine;
public class PlayerHealState : PlayerState
{
    private float entryTime;
    public PlayerHealState(PlayerController player, string stateName) : base(player, stateName) { }
    public override void Enter()
    {
        base.Enter();
        entryTime = Time.time;
        player.SetVelocity(0, 0);
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= entryTime + player.stats.healingDuration)
        {
            player.StatsManager.UseHealCharge();
            player.Health.Heal(player.stats.healAmount);
            stateMachine.ChangeState(player.IdleState);
        }
    }
}