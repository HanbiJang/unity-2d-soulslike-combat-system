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
        enemy.IsSuperArmor = false;
    }

    public override void Exit()
    {
        base.Exit();
        // ChaseState로 돌아가는 찰나를 보호
        enemy.IsSuperArmor = true;
        enemy.superArmorEndTime = float.MaxValue;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= hitStartTime + hitDuration)
        {
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



