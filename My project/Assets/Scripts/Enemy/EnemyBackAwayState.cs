using UnityEngine;

public class EnemyBackAwayState : EnemyState
{
    private float backAwayStartTime;
    private float backAwayDuration = 1f;

    public EnemyBackAwayState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        backAwayStartTime = Time.time;
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

        // 플레이어가 가드를 풀면 다시 추적
        if (!enemy.IsPlayerDefending())
        {
            enemy.ResetActionDelay();
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // 일정 시간 뒤로 물러난 후 다시 추적
        if (Time.time >= backAwayStartTime + backAwayDuration)
        {
            enemy.ResetActionDelay();
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        // 지면에 닿아있을 때만 이동
        if (!enemy.IsGrounded) return;
        
        // 플레이어 반대 방향으로 이동
        if (enemy.playerTarget != null)
        {
            Vector2 direction = (enemy.transform.position - enemy.playerTarget.position).normalized;
            float moveDirection = direction.x;
            enemy.CheckAndFlip(moveDirection);
            enemy.SetVelocityX(moveDirection * enemy.moveSpeed * 0.5f);
        }
    }
}



