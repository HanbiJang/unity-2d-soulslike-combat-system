using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
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

        float distance = enemy.GetDistanceToPlayer();

        // 우선순위 1: 플레이어가 회복 시도하면 → 돌진
        if (enemy.IsPlayerHealing())
        {
            enemy.lastRushTime = Time.time;
            stateMachine.ChangeState(enemy.RushState);
            return;
        }

        // 우선순위 2: 플레이어가 8m 이상 멀어지면 → 돌진
        if (distance >= enemy.rushTriggerDistance && Time.time >= enemy.lastRushTime + enemy.rushCooldown)
        {
            enemy.lastRushTime = Time.time;
            stateMachine.ChangeState(enemy.RushState);
            return;
        }

        // 우선순위 3: 플레이어가 가드 중이면 → 공격 안 함/뒤로 물러남
        if (enemy.IsPlayerDefending())
        {
            stateMachine.ChangeState(enemy.BackAwayState);
            return;
        }

        // 우선순위 4: 플레이어가 공격 범위 안(2m 이내) → 근접 공격
        if (distance <= enemy.attackRange)
        {
            // 플레이어가 가까이 있고 + 공격 중이면 → 패링
            if (enemy.IsPlayerAttacking() && distance <= 3f)
            {
                stateMachine.ChangeState(enemy.ParryState);
            }
            else
            {
                stateMachine.ChangeState(enemy.MeleeAttackState);
            }
            return;
        }

        // 우선순위 5: 플레이어가 10m 이상 멀어지면 → 원거리 공격
        if (distance >= enemy.rangedAttackDistance && Time.time >= enemy.lastRangedAttackTime + enemy.rangedAttackCooldown)
        {
            enemy.lastRangedAttackTime = Time.time;
            stateMachine.ChangeState(enemy.RangedAttackState);
            return;
        }

        // 그 외 → 추적 유지 (전투 중이면 계속 추적)
        if (enemy.isInCombat || distance <= enemy.detectionRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            // 감지 범위를 벗어나면 대기
            stateMachine.ChangeState(enemy.IdleState);
        }
    }

    private void MoveTowardsPlayer()
    {
        if (enemy.playerTarget == null) return;

        Vector2 direction = (enemy.playerTarget.position - enemy.transform.position).normalized;
        float moveDirection = direction.x;
        
        enemy.CheckAndFlip(moveDirection);
        enemy.SetVelocity(moveDirection * enemy.moveSpeed, enemy.Rb.velocity.y);
    }
}



