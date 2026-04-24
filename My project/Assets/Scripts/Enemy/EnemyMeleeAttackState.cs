using UnityEngine;

public class EnemyMeleeAttackState : EnemyState
{
    private float attackStartTime;
    private float attackDuration = 1f;
    private bool hasPerformedAttack = false;

    /// <summary>공격 타격 시점(초). 애니메이션 이벤트 없이 이 시점에 데미지 적용.</summary>
    private const float AttackHitTime = 0.35f;


    // 패링 가능 타이밍 설정
    private const float ParryWindowStart = 0.15f;  // 패링 가능 시작 시간
    private const float ParryWindowEnd = 0.6f;     // 패링 가능 종료 시간 (더 길게)
    private const float SlowMotionScale = 0.5f;    // 느려지는 정도 (0.5 = 50% 속도)
    private const float ColorTransitionSpeed = 15f; // 색상 변화 속도 (더 빠르게)
    
    private bool isInParryWindow = false;
    private float targetTimeScale = 1f;
    private Color targetColor = Color.white;
    private Color parryColor = new Color(1f, 0.3f, 0.3f, 1f); // 붉은 색

    // 1페이즈 공격 애니메이션 (일반 상태)
    private readonly string[] phase1Attacks = { "Attack1", "Attack2", "Attack3"};
    
    // 2페이즈 공격 애니메이션 (각성 상태)
    private readonly string[] phase2Attacks = { "Attack1_2", "Attack2+2", "Attack3_2", "JumpAttack_2" };
    
    private string currentAttackAnimation;

    public EnemyMeleeAttackState(EnemyController enemy, string stateName) : base(enemy, stateName) { }

    public override void Enter()
    {
        // base.Enter()를 호출하지 않음 - 랜덤 애니메이션 선택으로 대체
        attackStartTime = Time.time;
        hasPerformedAttack = false;
        isInParryWindow = false;
        targetTimeScale = 1f;
        targetColor = Color.white;
        enemy.SetVelocityX(0);
        enemy.IsSuperArmor = true;
        enemy.superArmorEndTime = float.MaxValue;

        // 플레이어 방향으로 바라보기
        if (enemy.playerTarget != null)
        {
            float direction = (enemy.playerTarget.position - enemy.transform.position).x;
            enemy.CheckAndFlip(direction);
        }

        // 공격 사운드 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.EnemyAttack);
        }

        // 페이즈에 따라 랜덤 공격 애니메이션 선택 및 재생
        SelectRandomAttackAnimation();
    }

    /// <summary>
    /// 보스의 현재 페이즈에 따라 랜덤 공격 애니메이션을 선택하고 재생합니다.
    /// </summary>
    private void SelectRandomAttackAnimation()
    {
        string[] attackPool;
        
        // 각성 상태(2페이즈)인지 확인
        if (enemy.isEnraged)
        {
            attackPool = phase2Attacks;
            Debug.Log("보스 2페이즈 공격 - 공격 4가지 중 선택");
        }
        else
        {
            attackPool = phase1Attacks;
            Debug.Log("보스 1페이즈 공격 - 공격 3가지 중 선택");
        }

        // 랜덤으로 공격 애니메이션 선택
        int randomIndex = Random.Range(0, attackPool.Length);
        currentAttackAnimation = attackPool[randomIndex];
        
        // 선택된 애니메이션 재생
        if (enemy.Anim != null)
        {
            enemy.Anim.Play(currentAttackAnimation);
            Debug.Log($"보스 공격 애니메이션 재생: {currentAttackAnimation}");
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 플레이어가 죽었으면 대기
        if (enemy.IsPlayerDead())
        {
            ResetTimeAndColor();
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        float elapsed = Time.time - attackStartTime;

        // 패링 가능 윈도우 체크
        bool wasInParryWindow = isInParryWindow;
        isInParryWindow = elapsed >= ParryWindowStart && elapsed <= ParryWindowEnd;
        
        // 패링 윈도우 진입/종료 처리
        if (isInParryWindow && !wasInParryWindow)
        {
            // 패링 윈도우 시작 - 즉시 적용
            targetTimeScale = SlowMotionScale;
            targetColor = parryColor;
            Time.timeScale = SlowMotionScale; // 슬로우 모션 즉시 적용
            SlowMotionEffects.Instance?.SetSlowMotion(true);
            Debug.Log("패링 윈도우 시작!");
        }
        else if (!isInParryWindow && wasInParryWindow)
        {
            // 패링 윈도우 종료
            targetTimeScale = 1f;
            targetColor = Color.white;
            SlowMotionEffects.Instance?.SetSlowMotion(false);
            Debug.Log("패링 윈도우 종료!");
        }
        
        // 패링 윈도우 중이면 색상을 부드럽게 전환
        if (isInParryWindow)
        {
            UpdateSpriteColor();
        }
        else
        {
            // 패링 윈도우가 아니면 시간과 색상을 부드럽게 복귀
            UpdateTimeScale();
            UpdateSpriteColor();
        }

        // 타이밍 기반 공격 판정 (애니메이션 이벤트 없이 칼 휘두르는 시점에 데미지)
        if (!hasPerformedAttack && elapsed >= AttackHitTime)
        {
            PerformAttack();
            hasPerformedAttack = true;
        }

        // 공격 시간이 지나면 추적 상태로
        if (elapsed >= attackDuration)
        {
            ResetTimeAndColor();
            enemy.ResetActionDelay();
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
            // #region agent log
            DebugLogUtil.Log("EnemyMeleeAttackState.AnimationTrigger", "AnimationTrigger called", "{\"hasPerformedAttack\":false}", "H1");
            // #endregion
            PerformAttack();
            hasPerformedAttack = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemy.IsSuperArmor = false;
        ResetTimeAndColor();
    }

    private void PerformAttack()
    {
        if (enemy.playerTarget == null)
        {
            // #region agent log
            DebugLogUtil.Log("EnemyMeleeAttackState.PerformAttack", "playerTarget is null", "{}", "H1");
            // #endregion
            return;
        }

        // 공격 범위 체크
        float distance = enemy.GetDistanceToPlayer();
        // #region agent log
        DebugLogUtil.Log("EnemyMeleeAttackState.PerformAttack", "Distance check", "{\"distance\":" + distance + ",\"attackRange\":" + enemy.attackRange + ",\"inRange\":" + (distance <= enemy.attackRange ? "true" : "false") + "}", "H2");
        // #endregion
        if (distance <= enemy.attackRange)
        {
            PlayerController player = enemy.playerTarget.GetComponent<PlayerController>();
            if (player != null && player.Health != null)
            {
                // #region agent log
                DebugLogUtil.Log("EnemyMeleeAttackState.PerformAttack", "Calling TakeDamage", "{\"damage\":" + enemy.attackDamage + ",\"playerHealth\":\"" + (player.Health != null ? "exists" : "null") + "\"}", "H1");
                // #endregion
                player.Health.TakeDamage(enemy.attackDamage, enemy.transform);
            }
            else
            {
                // #region agent log
                DebugLogUtil.Log("EnemyMeleeAttackState.PerformAttack", "Player or Health is null", "{\"player\":" + (player != null ? "exists" : "null") + ",\"health\":" + (player?.Health != null ? "exists" : "null") + "}", "H1");
                // #endregion
            }
        }
    }

    /// <summary>
    /// 시간 스케일을 부드럽게 전환합니다.
    /// </summary>
    private void UpdateTimeScale()
    {
        float currentTimeScale = Time.timeScale;
        float newTimeScale = Mathf.Lerp(currentTimeScale, targetTimeScale, Time.unscaledDeltaTime * ColorTransitionSpeed);
        Time.timeScale = newTimeScale;
    }

    /// <summary>
    /// 보스 스프라이트 색상을 부드럽게 전환합니다.
    /// </summary>
    private void UpdateSpriteColor()
    {
        if (enemy.SpriteRenderer != null)
        {
            enemy.SpriteRenderer.color = Color.Lerp(enemy.SpriteRenderer.color, targetColor, Time.unscaledDeltaTime * ColorTransitionSpeed);
        }
    }

    /// <summary>
    /// 시간 스케일과 색상을 즉시 원래대로 돌립니다.
    /// </summary>
    private void ResetTimeAndColor()
    {
        Time.timeScale = 1f;
        targetTimeScale = 1f;
        targetColor = Color.white;
        SlowMotionEffects.Instance?.SetSlowMotion(false);

        if (enemy.SpriteRenderer != null)
        {
            enemy.SpriteRenderer.color = Color.white;
        }
    }
}



