using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private float deathStartTime;
    private float deathDuration = 2f; // 사망 애니메이션 시간

    public EnemyDeathState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        deathStartTime = Time.time;
        enemy.SetVelocity(0, 0);
        
        Debug.Log(enemy.gameObject.name + "가 처치되었습니다.");
        
        // 보스인 경우 BossHUD에서 해제
        if (enemy.IsBoss)
        {
            BossHUD.Instance?.UnregisterBoss(enemy);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        // 사망 애니메이션 시간이 지나면 오브젝트 파괴
        if (Time.time >= deathStartTime + deathDuration)
        {
            Object.Destroy(enemy.gameObject);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemy.SetVelocity(0, 0);
    }
}

