using UnityEngine;

public class EnemyChaseState : EnemyState
{
    private Collider2D[] enemyColliders;
    private Collider2D[] playerColliders;
    private bool collisionIgnored = false;

    public EnemyChaseState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        
        // 플레이어와의 충돌 무시 (플레이어를 밀지 않음)
        enemyColliders = enemy.GetComponents<Collider2D>();
        if (enemy.playerTarget != null)
        {
            playerColliders = enemy.playerTarget.GetComponents<Collider2D>();
            
            if (enemyColliders != null && playerColliders != null)
            {
                foreach (var enemyCol in enemyColliders)
                {
                    if (enemyCol != null)
                    {
                        foreach (var playerCol in playerColliders)
                        {
                            if (playerCol != null)
                            {
                                Physics2D.IgnoreCollision(enemyCol, playerCol, true);
                            }
                        }
                    }
                }
                collisionIgnored = true;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // 플레이어와의 충돌 무시 해제 (보스는 항상 충돌 무시 유지)
        if (collisionIgnored && enemyColliders != null && playerColliders != null && !enemy.IsBoss)
        {
            foreach (var enemyCol in enemyColliders)
            {
                if (enemyCol != null)
                {
                    foreach (var playerCol in playerColliders)
                    {
                        if (playerCol != null)
                        {
                            Physics2D.IgnoreCollision(enemyCol, playerCol, false);
                        }
                    }
                }
            }
            collisionIgnored = false;
        }
        // 보스는 충돌 무시 상태 유지 (플레이어가 밀 수 없음)
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

        // 우선순위 1: 공격 범위 안 → 즉시 공격 (딜레이 없음)
        if (distance <= enemy.attackRange)
        {
            enemy.SetVelocityX(0f);
            enemy.IsSuperArmor = true;
            enemy.superArmorEndTime = float.MaxValue;
            if (enemy.IsPlayerAttacking() && distance <= 3f)
                stateMachine.ChangeState(enemy.ParryState);
            else
                stateMachine.ChangeState(enemy.MeleeAttackState);
            return;
        }

        // 행동 딜레이 체크 (특수 행동에만 적용)
        bool canAct = enemy.CanPerformAction();

        // 우선순위 2: 플레이어가 회복 시도하면 → 돌진 (딜레이 무시)
        if (enemy.IsPlayerHealing())
        {
            enemy.ResetActionDelay();
            enemy.lastRushTime = Time.time;
            stateMachine.ChangeState(enemy.RushState);
            return;
        }

        // 우선순위 3: 플레이어가 멀면 → 돌진
        if (canAct && distance >= enemy.rushTriggerDistance && Time.time >= enemy.lastRushTime + enemy.rushCooldown)
        {
            enemy.lastRushTime = Time.time;
            stateMachine.ChangeState(enemy.RushState);
            return;
        }

        // 우선순위 4: 플레이어가 가드 중이면 → 뒤로 물러남
        if (canAct && enemy.IsPlayerDefending())
        {
            stateMachine.ChangeState(enemy.BackAwayState);
            return;
        }

        // 우선순위 5: 플레이어가 멀면 → 원거리 공격
        if (canAct && distance >= enemy.rangedAttackDistance && Time.time >= enemy.lastRangedAttackTime + enemy.rangedAttackCooldown)
        {
            enemy.lastRangedAttackTime = Time.time;
            stateMachine.ChangeState(enemy.RangedAttackState);
            return;
        }

        // 그 외 → 추적 유지 (전투 중이면 계속 추적)
        // 단, 플레이어가 죽지 않았을 때만
        if (!enemy.IsPlayerDead() && (enemy.isInCombat || distance <= enemy.detectionRange))
        {
            MoveTowardsPlayer();
        }
        else
        {
            // 플레이어가 죽었거나 감지 범위를 벗어나면 대기
            stateMachine.ChangeState(enemy.IdleState);
        }
    }

    /// <summary>추적 시 플레이어 앞 attackRange 지점까지만 이동 (닿지 않도록).</summary>
    private const float ChaseStopEpsilon = 0.02f;

    private void MoveTowardsPlayer()
    {
        if (enemy.playerTarget == null) return;
        if (!enemy.IsGrounded) return;

        Vector2 enemyPos = enemy.transform.position;
        Vector2 playerPos = enemy.playerTarget.position;
        Vector2 toPlayer = (playerPos - enemyPos);
        float distance = toPlayer.magnitude;

        // 이미 attackRange 이내면 이동 안 함 (chase=공격 방지, 근접 전환은 LogicUpdate에서 처리)
        if (distance <= enemy.attackRange)
        {
            enemy.SetVelocityX(0f);
            return;
        }

        // 목표: 플레이어 바로 앞 attackRange 지점 (적↔플레이어 사이)
        Vector2 dir = toPlayer.normalized;
        Vector2 stopPoint = playerPos - dir * enemy.attackRange;
        Vector2 toStop = stopPoint - enemyPos;

        if (toStop.sqrMagnitude < ChaseStopEpsilon * ChaseStopEpsilon)
        {
            enemy.SetVelocityX(0f);
            return;
        }

        float moveX = toStop.normalized.x;
        enemy.CheckAndFlip(moveX);
        enemy.SetVelocityX(moveX * enemy.moveSpeed);
    }
}



