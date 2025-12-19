using UnityEngine;

public class EnemyController : Enemy
{

    [Header("AI 설정")]
    [Tooltip("플레이어 감지 범위")]
    public float detectionRange = 15f;
    
    [Tooltip("근접 공격 범위")]
    public float attackRange = 2f;
    
    [Tooltip("돌진 발동 거리")]
    public float rushTriggerDistance = 8f;
    
    [Tooltip("원거리 공격 발동 거리")]
    public float rangedAttackDistance = 10f;
    
    [Tooltip("돌진 쿨다운")]
    public float rushCooldown = 5f;
    
    [Tooltip("원거리 공격 쿨다운")]
    public float rangedAttackCooldown = 3f;
    
    [Tooltip("이동 속도")]
    public float moveSpeed = 5f;
    
    [Tooltip("돌진 속도")]
    public float rushSpeed = 15f;

    [Header("행동 딜레이 설정")]
    [Tooltip("행동 사이 최소 딜레이 시간")]
    public float minActionDelay = 0.5f;
    
    [Tooltip("행동 사이 최대 딜레이 시간")]
    public float maxActionDelay = 2.0f;

    [Header("컴포넌트")]
    [Tooltip("스프라이트와 애니메이터가 있는 자식 오브젝트")]
    [SerializeField] private Transform visuals;
    
    [Tooltip("지면 체크용 Transform (보스 발밑에 만들어주세요)")]
    [SerializeField] private Transform groundCheck;
    
    [Tooltip("지면 레이어")]
    [SerializeField] private LayerMask groundLayer;
    
    [Tooltip("지면 체크 거리")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    
    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    public EnemyStateMachine StateMachine { get; private set; }
    public bool IsGrounded { get; private set; }

    // 상태
    public EnemyIdleState IdleState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyRushState RushState { get; private set; }
    public EnemyMeleeAttackState MeleeAttackState { get; private set; }
    public EnemyRangedAttackState RangedAttackState { get; private set; }
    public EnemyBackAwayState BackAwayState { get; private set; }
    public EnemyParryState ParryState { get; private set; }
    public EnemyEnrageState EnrageState { get; private set; }
    public EnemyHitState HitState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }

    [Header("디버깅용")]
    [SerializeField] private string currentStateName;

    // 내부 변수
    public Transform playerTarget { get; private set; }
    private PlayerController playerController;
    public float lastRushTime { get; set; } = -10f;
    public float lastRangedAttackTime { get; set; } = -10f;
    public bool isInCombat { get; private set; } = false;
    public bool isEnraged { get; private set; } = false;
    public bool IsFacingRight { get; private set; } = true;
    
    // 행동 딜레이 시스템
    private float lastActionTime = -10f;
    private float nextActionDelay = 0f;

    // 체력 정보 접근용 프로퍼티 (Enemy에서 상속)
    public float CurrentHealthPercentage => (float)CurrentHealth / MaxHealth;

    protected override void Awake()
    {
        base.Awake();
        Rb = GetComponent<Rigidbody2D>();
        
        // GroundCheck 찾기
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                Debug.LogWarning("'GroundCheck' 자식 오브젝트를 찾을 수 없음! 보스 발밑에 만들어주세요.");
            }
        }
        
        // GroundLayer가 설정되지 않았으면 플레이어의 설정을 참고
        if (groundLayer.value == 0 && playerController != null)
        {
            // 나중에 Start에서 설정
        }
        
        if (visuals != null)
        {
            Anim = visuals.GetComponent<Animator>();
        }
        else
        {
            Anim = GetComponentInChildren<Animator>();
        }

        // 플레이어 찾기
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerTarget = playerController.transform;
            // 플레이어의 groundLayer를 참고
            if (groundLayer.value == 0 && playerController.stats != null)
            {
                groundLayer = playerController.stats.groundLayer;
            }
        }

        // 상태 머신 초기화
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, "IDLE");
        ChaseState = new EnemyChaseState(this, "CHASE");
        RushState = new EnemyRushState(this, "RUSH");
        MeleeAttackState = new EnemyMeleeAttackState(this, "MELEE_ATTACK");
        RangedAttackState = new EnemyRangedAttackState(this, "RANGED_ATTACK");
        BackAwayState = new EnemyBackAwayState(this, "BACK_AWAY");
        ParryState = new EnemyParryState(this, "PARRY");
        EnrageState = new EnemyEnrageState(this, "ENRAGE");
        HitState = new EnemyHitState(this, "HIT");
        DeathState = new EnemyDeathState(this, "DEATH");
    }

    protected override void Start()
    {
        base.Start(); // Enemy의 Start 호출 (BossHUD 등록 등)

        // 초기 상태 설정
        StateMachine.Initialize(IdleState);
    }

    void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        currentStateName = StateMachine.CurrentState.stateName;
    }

    void FixedUpdate()
    {
        CheckGrounded();
        StateMachine.CurrentState.PhysicsUpdate();
    }
    
    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            IsGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        }
        else
        {
            // GroundCheck가 없으면 항상 지면에 있다고 가정 (공중 이동 방지)
            IsGrounded = true;
        }
    }

    public void AnimationTrigger()
    {
        StateMachine.CurrentState.AnimationTrigger();
    }

    // 플레이어 감지 헬퍼 메서드들
    public float GetDistanceToPlayer()
    {
        if (playerTarget == null) return float.MaxValue;
        return Vector2.Distance(transform.position, playerTarget.position);
    }

    public bool IsPlayerInRange(float minRange, float maxRange)
    {
        float distance = GetDistanceToPlayer();
        return distance >= minRange && distance <= maxRange;
    }

    public bool IsPlayerOutOfRange(float range)
    {
        return GetDistanceToPlayer() > range;
    }

    public bool IsPlayerInState<T>() where T : PlayerState
    {
        if (playerController == null) return false;
        return playerController.StateMachine.CurrentState is T;
    }

    public bool IsPlayerHealing()
    {
        return IsPlayerInState<PlayerHealState>();
    }

    public bool IsPlayerDefending()
    {
        return IsPlayerInState<PlayerDefendState>();
    }

    public bool IsPlayerAttacking()
    {
        return IsPlayerInState<PlayerAttackState>() || 
               IsPlayerInState<PlayerAirAttackState>() ||
               IsPlayerInState<PlayerDashAttackState>() ||
               IsPlayerInState<PlayerSpecialAttackState>();
    }

    public bool IsPlayerInAir()
    {
        if (playerController == null) return false;
        return !playerController.IsGrounded;
    }

    public bool IsPlayerDashing()
    {
        return IsPlayerInState<PlayerDashState>();
    }

    public bool IsPlayerHit()
    {
        return IsPlayerInState<PlayerHitState>();
    }

    public bool IsPlayerDead()
    {
        return IsPlayerInState<PlayerDeathState>();
    }

    // 전투 시작 (플레이어가 타격을 입었을 때 호출)
    public void StartCombat()
    {
        isInCombat = true;
    }

    // 행동 딜레이 체크 (랜덤 딜레이가 지났는지 확인)
    public bool CanPerformAction()
    {
        // 딜레이가 설정되지 않았으면 설정하고 즉시 허용 (첫 행동은 즉시 실행)
        if (nextActionDelay <= 0f)
        {
            nextActionDelay = Random.Range(minActionDelay, maxActionDelay);
            lastActionTime = Time.time;
            return true;  // 첫 행동은 즉시 허용
        }

        // 딜레이가 지났는지 확인
        if (Time.time >= lastActionTime + nextActionDelay)
        {
            // 다음 딜레이를 랜덤으로 설정 (엇박 효과)
            nextActionDelay = Random.Range(minActionDelay, maxActionDelay);
            lastActionTime = Time.time;
            return true;
        }

        return false;
    }

    // 행동 실행 후 딜레이 리셋 (긴급 상황용)
    public void ResetActionDelay()
    {
        nextActionDelay = 0f;
        lastActionTime = Time.time;
    }

    // 이동 관련
    public void SetVelocity(float x, float y)
    {
        if (Rb != null)
        {
            // 지면에 닿아있을 때만 Y 속도를 설정 (공중에서는 중력에 맡김)
            if (IsGrounded)
            {
                Rb.velocity = new Vector2(x, y);
            }
            else
            {
                // 공중에서는 X 속도만 설정, Y는 중력에 맡김
                Rb.velocity = new Vector2(x, Rb.velocity.y);
            }
        }
    }
    
    // X 속도만 설정 (Y는 유지)
    public void SetVelocityX(float x)
    {
        if (Rb != null)
        {
            Rb.velocity = new Vector2(x, Rb.velocity.y);
        }
    }

    public void CheckAndFlip(float direction)
    {
        if (direction > 0 && !IsFacingRight)
        {
            Flip();
        }
        else if (direction < 0 && IsFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        
        if (visuals != null)
        {
            Vector3 scale = visuals.localScale;
            scale.x *= -1;
            visuals.localScale = scale;
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // 피해 받기 (Enemy의 TakeDamage를 오버라이드)
    public override void TakeDamage(int damage)
    {
        // 전투 시작
        if (!isInCombat)
        {
            StartCombat();
        }

        // 피격 상태로 전환
        if (StateMachine.CurrentState != HitState && StateMachine.CurrentState != DeathState)
        {
            StateMachine.ChangeState(HitState);
        }

        // 체력 감소 (부모의 TakeDamage는 호출하지 않음 - Die() 호출을 막기 위해)
        currentHealth -= damage;
        Debug.Log(gameObject.name + "가 " + damage + "의 피해를 입었습니다! 현재 체력: " + currentHealth);

        // FlashRed 효과
        if (sr != null)
        {
            StartCoroutine(FlashRedCoroutine());
        }

        // 체력이 50% 이하로 떨어지면 분노 상태
        if (CurrentHealthPercentage <= 0.5f && !isEnraged)
        {
            isEnraged = true;
            if (StateMachine.CurrentState != HitState && StateMachine.CurrentState != DeathState)
            {
                StateMachine.ChangeState(EnrageState);
            }
        }

        // 사망 처리 (상태 머신으로 처리)
        if (currentHealth <= 0 && StateMachine.CurrentState != DeathState)
        {
            StateMachine.ChangeState(DeathState);
        }
    }

    private System.Collections.IEnumerator FlashRedCoroutine()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }

    // Die 오버라이드 (상태 머신으로 처리)
    protected override void Die()
    {
        // Enemy의 Die는 호출하지 않고 상태 머신으로 처리
        if (StateMachine.CurrentState != DeathState)
        {
            StateMachine.ChangeState(DeathState);
        }
    }
}

