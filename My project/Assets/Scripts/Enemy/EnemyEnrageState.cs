using UnityEngine;

public class EnemyEnrageState : EnemyState
{
    private float enrageStartTime;
    private float enrageDuration = 2f;
    private float originalMoveSpeed;
    private float originalRushSpeed;

    public EnemyEnrageState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        enrageStartTime = Time.time;

        // 분노 상태: 이동 속도와 공격 속도 증가
        originalMoveSpeed = enemy.moveSpeed;
        originalRushSpeed = enemy.rushSpeed;
        enemy.moveSpeed *= 1.5f;
        enemy.rushSpeed *= 1.3f;
        enemy.rushCooldown *= 0.7f; // 쿨다운 감소

        Debug.Log("보스 분노 상태 전환! (2페이즈)");
    }

    public override void Exit()
    {
        base.Exit();
        // 분노 상태 종료 시 원래 속도로 복구하지 않음 (계속 유지)
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 분노 상태 애니메이션/이펙트 시간이 지나면 추적 상태로
        if (Time.time >= enrageStartTime + enrageDuration)
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }
    }
}



