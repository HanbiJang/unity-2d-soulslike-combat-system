using UnityEngine;

public class EnemyGroggyState : EnemyState
{
    private float groggyStartTime;

    public EnemyGroggyState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        // base.Enter() 대신 직접 애니메이션 재생 (별도 GROGGY 애니메이션)
        groggyStartTime = Time.time;
        enemy.SetVelocityX(0);
        enemy.IsSuperArmor = false;

        if (enemy.Anim != null)
            enemy.Anim.Play("Groggy");

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundType.EnemyHit);
    }

    public override void LogicUpdate()
    {
        // 사망 체크
        if (enemy.CurrentHealth <= 0)
            return;

        if (Time.time >= groggyStartTime + enemy.groggyDuration)
        {
            enemy.ResetPosture();

            if (enemy.isEnraged && !enemy.hasPlayedEnrageAnimation)
            {
                enemy.hasPlayedEnrageAnimation = true;
                stateMachine.ChangeState(enemy.EnrageState);
            }
            else
            {
                stateMachine.ChangeState(enemy.ChaseState);
            }
        }
    }
}
