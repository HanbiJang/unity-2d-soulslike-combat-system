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

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(SoundType.EnemyAttack);

        if (enemy.projectilePrefab == null)
        {
            Debug.LogWarning("EnemyController에 projectilePrefab이 설정되지 않았습니다!");
            return;
        }

        Vector3 spawnPos = enemy.transform.position + new Vector3(enemy.IsFacingRight ? 0.5f : -0.5f, 0.3f, 0f);
        GameObject obj = Object.Instantiate(enemy.projectilePrefab, spawnPos, Quaternion.identity);

        EnemyProjectile projectile = obj.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            Vector2 dir = (enemy.playerTarget.position - enemy.transform.position).normalized;
            projectile.damage = enemy.attackDamage;

            Collider2D[] enemyColliders = enemy.GetComponents<Collider2D>();
            PlayerStatsSO stats = enemy.playerTarget.GetComponent<PlayerController>()?.stats;
            LayerMask ground = stats != null ? stats.groundLayer : default;
            projectile.Launch(dir, enemyColliders, ground);
        }
    }
}



