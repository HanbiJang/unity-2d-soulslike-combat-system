using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        enemy.SetVelocityX(0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.IsPlayerDead()) return;

        // 전투 중이면 추적 상태로
        if (enemy.isInCombat)
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }
    }
}



