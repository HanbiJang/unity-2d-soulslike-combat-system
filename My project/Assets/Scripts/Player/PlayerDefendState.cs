using UnityEngine;
public class PlayerDefendState : PlayerState
{
    private float entryTime;
    private bool isParrying;

    public PlayerDefendState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        entryTime = Time.time;
        isParrying = true; player.SetVelocity(0, 0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isParrying && Time.time >= entryTime + player.stats.parryWindow)
        {
            isParrying = false;
            Debug.Log("패링 시간 종료. 이제부터 일반 방어입니다.");
        }

        if (!player.IsDefendInput)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public void HandleDamage(int damage, Transform damageSource)
    {
        if (isParrying)
        {
            Debug.Log("패링 성공!");
            // 패링 성공 시 피해 없음
            return;
        }
        
        // 가드 시: 공격의 10% 데미지 + 공격의 90%가 스태미나로 감소
        int healthDamage = Mathf.RoundToInt(damage * 0.1f);  // 10% 체력 피해
        float staminaDamage = damage * 0.9f;  // 90% 스태미나 피해
        
        // 체력 피해 처리
        if (healthDamage > 0)
        {
            player.Health.TakeDamageDirectly(healthDamage);
        }
        
        // 스태미나 피해 처리
        if (staminaDamage > 0)
        {
            player.StatsManager.UseStamina(staminaDamage);
        }
        
        Debug.Log($"방어 성공! 체력: -{healthDamage}, 스태미나: -{staminaDamage:F1}");
    }
}