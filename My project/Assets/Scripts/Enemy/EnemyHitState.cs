using UnityEngine;

public class EnemyHitState : EnemyState
{
    private float hitStartTime;
    private float hitDuration = 0.5f;

    public EnemyHitState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        hitStartTime = Time.time;
        enemy.SetVelocityX(0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 피격 시간이 지나면 추적 상태로
        if (Time.time >= hitStartTime + hitDuration)
        {
            // 분노 상태가 되어야 하면 분노 상태로
            if (enemy.isEnraged && enemy.StateMachine.CurrentState != enemy.EnrageState)
            {
                stateMachine.ChangeState(enemy.EnrageState);
            }
            else
            {
                stateMachine.ChangeState(enemy.ChaseState);
            }
            return;
        }
    }
}



