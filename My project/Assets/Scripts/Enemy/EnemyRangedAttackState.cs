using UnityEngine;

public class EnemyRangedAttackState : EnemyState
{
    private float attackStartTime;
    private float attackDuration = 1.5f;
    private bool hasPerformedAttack = false;

    public EnemyRangedAttackState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        base.Enter();
        attackStartTime = Time.time;
        hasPerformedAttack = false;
        enemy.SetVelocityX(0);

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

        // 플레이어가 죽었으면 대기
        if (enemy.IsPlayerDead())
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        // 공격 시간이 지나면 추적 상태로
        if (Time.time >= attackStartTime + attackDuration)
        {
            // 공격 후 딜레이 리셋 (다음 행동까지 텀을 둠)
            enemy.ResetActionDelay();
            stateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // 원거리 공격 중 30% 확률로 돌진 발동 (선택적)
        // if (Random.value < 0.3f && Time.time >= enemy.lastRushTime + enemy.rushCooldown)
        // {
        //     enemy.lastRushTime = Time.time;
        //     stateMachine.ChangeState(enemy.RushState);
        //     return;
        // }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        
        // 원거리 공격 발사 (한 번만 실행)
        if (!hasPerformedAttack)
        {
            PerformRangedAttack();
            hasPerformedAttack = true;
        }
    }

    private void PerformRangedAttack()
    {
        if (enemy.playerTarget == null) return;

        // 원거리 공격 사운드 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.EnemyAttack);
        }

        // 원거리 투사체 발사 로직
        // 여기에 투사체 생성 코드 추가
        // 예: Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        Debug.Log("원거리 공격 발사!");
    }
}



