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
        Debug.Log($"[HitState.Enter] t={Time.time:F2} | IsSuperArmor=false | consecutiveHitCount(내부값은 TakeDamage 참고)");
    }

    public override void Exit()
    {
        base.Exit();
        enemy.ResetActionDelay();
        Debug.Log($"[HitState.Exit] t={Time.time:F2} | IsSuperArmor={enemy.IsSuperArmor} | superArmorEndTime={enemy.superArmorEndTime:F2} | → ChaseState 예정");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= hitStartTime + hitDuration)
        {
            if (enemy.isEnraged && !enemy.hasPlayedEnrageAnimation)
            {
                enemy.hasPlayedEnrageAnimation = true;
                Debug.Log($"[HitState] t={Time.time:F2} | 각성 미재생 → EnrageState");
                stateMachine.ChangeState(enemy.EnrageState);
            }
            else
            {
                Debug.Log($"[HitState] t={Time.time:F2} | hitDuration 완료 → ChaseState");
                stateMachine.ChangeState(enemy.ChaseState);
            }
            return;
        }
    }
}


