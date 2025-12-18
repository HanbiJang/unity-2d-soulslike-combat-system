using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocity(0, enemy.Rb.velocity.y);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 전투 중이면 추적 상태로
        if (enemy.isInCombat)
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // 플레이어가 감지 범위 안에 들어오면 추적 시작
        if (enemy.GetDistanceToPlayer() <= enemy.detectionRange)
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }
    }
}



