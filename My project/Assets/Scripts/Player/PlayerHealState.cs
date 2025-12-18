using UnityEngine;
public class PlayerHealState : PlayerState
{
    private float entryTime;
    private bool isInterrupted = false;
    private bool hasUsedCharge = false;

    public PlayerHealState(PlayerController player, string stateName) : base(player, stateName) { }
    
    public override void Enter()
    {
        base.Enter();
        entryTime = Time.time;
        isInterrupted = false;
        hasUsedCharge = false;
        player.SetVelocity(0, 0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        // 회복이 중단되었는지 확인
        if (isInterrupted)
        {
            // 중단되었지만 아직 물약을 소모하지 않았다면 소모
            if (!hasUsedCharge)
            {
                player.StatsManager.UseHealCharge();
                hasUsedCharge = true;
            }
            // HitState로 전환 (PlayerHealth에서 처리됨)
            return;
        }

        // 회복 완료
        if (Time.time >= entryTime + player.stats.healingDuration)
        {
            player.StatsManager.UseHealCharge();
            player.Health.Heal(player.stats.healAmount);
            hasUsedCharge = true;
            
            // 적절한 상태로 전환
            if (player.IsGrounded)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // 회복 중에는 움직이지 않음
        player.SetVelocity(0, player.Rb.velocity.y);
    }

    // 회복 중단 메서드 (PlayerHealth에서 호출)
    public void InterruptHealing()
    {
        isInterrupted = true;
    }
}