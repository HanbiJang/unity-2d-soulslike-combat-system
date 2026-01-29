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
            // 각성 상태가 되어야 하고, 각성 애니메이션을 아직 재생하지 않았으면 각성 애니메이션 재생
            if (enemy.isEnraged && !enemy.hasPlayedEnrageAnimation)
            {
                enemy.hasPlayedEnrageAnimation = true;
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



