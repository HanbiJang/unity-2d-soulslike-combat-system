using UnityEngine;

public class EnemyRushState : EnemyState
{
    private float rushStartTime;
    private float rushDuration = 1.5f;
    private Vector2 rushDirection;

    public EnemyRushState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        rushStartTime = Time.time;

        // 플레이어 방향으로 돌진
        if (enemy.playerTarget != null)
        {
            rushDirection = (enemy.playerTarget.position - enemy.transform.position).normalized;
            float moveDirection = rushDirection.x;
            enemy.CheckAndFlip(moveDirection);
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



