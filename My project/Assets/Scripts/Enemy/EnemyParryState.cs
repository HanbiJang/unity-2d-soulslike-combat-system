using UnityEngine;

public class EnemyParryState : EnemyState
{
    private float parryStartTime;
    private float parryWindow = 0.3f;
    private bool hasParried = false;

    public EnemyParryState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        parryStartTime = Time.time;
        hasParried = false;
        enemy.SetVelocityX(0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 플레이어가 죽었으면 대기
        if (enemy.IsPlayerDead())
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        // 패링 시간이 지나면 추적 상태로
        if (Time.time >= parryStartTime + parryWindow)
        {
            if (hasParried)
            {
                // 패링 성공 후 반격 (즉시 반격, 딜레이 무시)
                stateMachine.ChangeState(enemy.MeleeAttackState);
            }
            else
            {
                // 패링 실패
                enemy.ResetActionDelay();
                stateMachine.ChangeState(enemy.ChaseState);
            }
            return;
        }

        // 플레이어가 공격 중이고 패링 윈도우 안에 있으면 패링 성공
        if (enemy.IsPlayerAttacking() && !hasParried)
        {
            hasParried = true;
            Debug.Log("보스 패링 성공!");
            
            // 플레이어를 경직시키거나 넉백
            if (enemy.playerTarget != null)
            {
                PlayerController player = enemy.playerTarget.GetComponent<PlayerController>();
                if (player != null && player.Health != null)
                {
                    // 패링 성공 시 플레이어에게 약간의 피해 또는 경직
                    // player.Health.TakeDamage(1, enemy.transform);
                }
            }
        }
    }
}



