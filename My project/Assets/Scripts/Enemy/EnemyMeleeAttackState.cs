using UnityEngine;

public class EnemyMeleeAttackState : EnemyState
{
    private float attackStartTime;
    private float attackDuration = 1f;
    private bool hasPerformedAttack = false;

    public EnemyMeleeAttackState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        attackStartTime = Time.time;
        hasPerformedAttack = false;
        enemy.SetVelocity(0, enemy.Rb.velocity.y);

        // 플레이어 방향으로 바라보기
        if (enemy.playerTarget != null)
        {
            float direction = (enemy.playerTarget.position - enemy.transform.position).x;
            enemy.CheckAndFlip(direction);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 공격 중에는 플레이어가 가드 중이면 공격 취소하지 않음 (이미 공격 시작)
        // 공격이 끝나면 다시 체크

        // 공격 시간이 지나면 추적 상태로
        if (Time.time >= attackStartTime + attackDuration)
        {
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        
        // 공격 판정 (한 번만 실행)
        if (!hasPerformedAttack)
        {
            PerformAttack();
            hasPerformedAttack = true;
        }
    }

    private void PerformAttack()
    {
        if (enemy.playerTarget == null) return;

        // 공격 범위 체크
        float distance = enemy.GetDistanceToPlayer();
        if (distance <= enemy.attackRange)
        {
            PlayerController player = enemy.playerTarget.GetComponent<PlayerController>();
            if (player != null && player.Health != null)
            {
                player.Health.TakeDamage(enemy.attackDamage, enemy.transform);
            }
        }
    }
}



