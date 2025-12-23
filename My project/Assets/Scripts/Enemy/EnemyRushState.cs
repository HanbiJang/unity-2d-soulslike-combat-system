using UnityEngine;

public class EnemyRushState : EnemyState
{
    private float rushStartTime;
    private float rushDuration = 1.5f;
    private Vector2 rushDirection;
    private Collider2D[] enemyColliders;
    private Collider2D[] playerColliders;
    private bool collisionIgnored = false;

    public EnemyRushState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        rushStartTime = Time.time;

        // 플레이어와의 충돌만 무시 (지면 충돌은 유지)
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

        // 플레이어 방향으로 돌진
        if (enemy.playerTarget != null)
        {
            rushDirection = (enemy.playerTarget.position - enemy.transform.position).normalized;
            float moveDirection = rushDirection.x;
            enemy.CheckAndFlip(moveDirection);
        }
    }

    public override void Exit()
    {
        base.Exit();
        
        // 플레이어와의 충돌 무시 해제
        if (collisionIgnored && enemyColliders != null && playerColliders != null)
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

        // 돌진 시간이 지나면 추적 상태로
        if (Time.time >= rushStartTime + rushDuration)
        {
            // 돌진 후 딜레이 리셋
            enemy.ResetActionDelay();
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // 플레이어에게 도달하면 근접 공격 (딜레이 무시 - 돌진 중이므로)
        if (enemy.GetDistanceToPlayer() <= enemy.attackRange)
        {
            stateMachine.ChangeState(enemy.MeleeAttackState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        // 지면에 닿아있을 때만 돌진
        if (!enemy.IsGrounded) return;
        
        // 돌진 이동
        if (enemy.playerTarget != null)
        {
            rushDirection = (enemy.playerTarget.position - enemy.transform.position).normalized;
            float moveDirection = rushDirection.x;
            enemy.CheckAndFlip(moveDirection);
        }
        
        enemy.SetVelocityX(rushDirection.x * enemy.rushSpeed);
    }
}



