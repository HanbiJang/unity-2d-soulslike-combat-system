using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    [Header("핵심 데이터")]
    [SerializeField] public PlayerStatsSO stats;

    [Header("컴포넌트")]
    [Tooltip("스프라이트와 애니메이터가 있는 자식 오브젝트")]
    [SerializeField] private Transform visuals;
    public PlayerStatsManager StatsManager { get; private set; }
    public Transform WallCheck { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerDefendState DefendState { get; private set; }
    public PlayerHealState HealState { get; private set; }
    public PlayerThrowState ThrowState { get; private set; }
    public PlayerSpecialAttackState SpecialAttackState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerClimbState ClimbState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerDashAttackState DashAttackState { get; private set; }
    public PlayerAirAttackState AirAttackState { get; private set; }
    public PlayerCrouchState CrouchState { get; private set; }

    public bool IsTouchingWall { get; private set; }
    public bool IsRunInput { get; private set; }
    public bool IsDefendInput { get; private set; }
    public bool IsHealInput { get; private set; }
    public bool IsThrowInput { get; private set; }
    public bool IsSpecialAttackInput { get; private set; }
    public Vector2 Input { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool IsFacingRight { get; private set; } = true;
    public bool IsGrounded { get; private set; }
    public bool IsTouchingLadder { get; private set; }
    public bool IsInvincible { get; set; }
    public int ComboCounter { get; set; }
    public bool IsAttackInputBuffered { get; set; }

    public Rigidbody2D Rb { get; private set; }
    public Animator Anim { get; private set; }
    public Transform GroundCheck { get; private set; }
    public CapsuleCollider2D PlayerCollider { get; private set; }
    public PlayerHealth Health { get; private set; }

    [Header("디버깅용")]
    [SerializeField] private string currentStateName;
    public PlayerStateMachine StateMachine { get; private set; }
    private float defaultGravityScale;
    public float lastDashTime { get; set; }
    public float lastAttackTime { get; set; }
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private Vector3 originalVisualsPosition; private Vector3 originalVisualsScale;


    public void AnimationTrigger()
    {
        StateMachine.CurrentState.AnimationTrigger();
    }

    private void Awake()
    {
        GroundCheck = transform.Find("GroundCheck");
        if (GroundCheck == null)
        {
            Debug.LogError("'GroundCheck' 자식 오브젝트를 찾을 수 없음! 플레이어 발밑에 만들어주세요.");
        }

        Rb = GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<CapsuleCollider2D>(); Health = GetComponent<PlayerHealth>();
        StatsManager = GetComponent<PlayerStatsManager>();
        WallCheck = transform.Find("WallCheck");

        if (visuals != null)
        {
            Anim = visuals.GetComponent<Animator>();
            originalVisualsPosition = visuals.localPosition;
            originalVisualsScale = visuals.localScale;
        }
        else
        {
            Debug.LogError("'Visuals' 연결 안 됨! 인스펙터에서 연결 필요");
        }

        originalColliderSize = PlayerCollider.size;
        originalColliderOffset = PlayerCollider.offset;
        defaultGravityScale = Rb.gravityScale;
        lastDashTime = -10f; lastAttackTime = -10f;


        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, "IDLE");
        MoveState = new PlayerMoveState(this, "WALK");
        JumpState = new PlayerJumpState(this, "JUMP");
        InAirState = new PlayerInAirState(this, "JUMP-FALL");
        DashState = new PlayerDashState(this, "DASH");
        DashAttackState = new PlayerDashAttackState(this, "ATTACK 1");
        AttackState = new PlayerAttackState(this, "ATTACK");
        HitState = new PlayerHitState(this, "HURT");
        CrouchState = new PlayerCrouchState(this, "crouch");
        DeathState = new PlayerDeathState(this, "DEATH 0");
        AirAttackState = new PlayerAirAttackState(this, "AIR ATTACK");
        RunState = new PlayerRunState(this, "RUN");
        DefendState = new PlayerDefendState(this, "DEFEND");
        HealState = new PlayerHealState(this, "HEALING");
        ThrowState = new PlayerThrowState(this, "THROW");
        SpecialAttackState = new PlayerSpecialAttackState(this, "SPECIAL ATTACK");
        WallSlideState = new PlayerWallSlideState(this, "WALL SLIDE");
        WallJumpState = new PlayerWallJumpState(this, "WALL JUMP");
    }

    private void Start()
    {
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        Input = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
        if (UnityEngine.Input.GetButtonDown("Jump")) StartCoroutine(JumpInputStopRoutine());
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift)) StartCoroutine(DashInputStopRoutine());
        if (UnityEngine.Input.GetKeyDown(KeyCode.Z) || UnityEngine.Input.GetMouseButtonDown(0)) StartCoroutine(AttackInputStopRoutine());

        IsRunInput = UnityEngine.Input.GetKey(KeyCode.LeftShift);
        IsDefendInput = UnityEngine.Input.GetKey(KeyCode.Mouse1); IsHealInput = UnityEngine.Input.GetKeyDown(KeyCode.R);
        IsThrowInput = UnityEngine.Input.GetKeyDown(KeyCode.F);
        IsSpecialAttackInput = UnityEngine.Input.GetKeyDown(KeyCode.Q);

        StateMachine.CurrentState.LogicUpdate();
        currentStateName = StateMachine.CurrentState.stateName;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckLadder();
        CheckWall();
        StateMachine.CurrentState.PhysicsUpdate();
    }

    private void CheckWall()
    {
        if (WallCheck != null)
        {
            IsTouchingWall = Physics2D.Raycast(WallCheck.position, Vector2.right * (IsFacingRight ? 1 : -1), stats.wallCheckDistance, stats.wallLayer);
        }
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics2D.Raycast(GroundCheck.position, Vector2.down, stats.groundCheckDistance, stats.groundLayer);
    }

    private void CheckLadder()
    {
        IsTouchingLadder = PlayerCollider.IsTouchingLayers(stats.ladderLayer);
    }

    public bool CanDash()
    {
        return Time.time >= lastDashTime + stats.dashCooldown;
    }

    public bool CanAttack()
    {
        if (Time.time >= lastAttackTime + stats.comboResetTime)
        {
            ComboCounter = 0;
        }
        return Time.time >= lastAttackTime + stats.attackCooldown;
    }


    public void PerformAttack(bool isGroundedAttack, AttackData attackData)
    {
        if (stats.enemyLayer.value == 0)
        {
            Debug.LogWarning("PlayerStatsSO에 Enemy Layer가 설정 안됨!");
            return;
        }

        int damage = isGroundedAttack ? attackData.groundAttackDamage : attackData.airAttackDamage;
        Vector2 attackPointOffset = isGroundedAttack ? attackData.groundAttackPointOffset : attackData.airAttackPointOffset;
        Vector2 attackBoxSize = isGroundedAttack ? attackData.groundAttackBoxSize : attackData.airAttackBoxSize;
        Vector2 attackPointPos = Rb.position + new Vector2(attackPointOffset.x * (IsFacingRight ? 1 : -1), attackPointOffset.y);

        RaycastHit2D[] hitEnemies = Physics2D.BoxCastAll(attackPointPos, attackBoxSize, 0f, Vector2.zero, 0, stats.enemyLayer);

        HashSet<Collider2D> processedEnemies = new HashSet<Collider2D>();
        foreach (RaycastHit2D hit in hitEnemies)
        {
            if (!processedEnemies.Contains(hit.collider))
            {
                ProcessHit(hit.collider, damage);
                processedEnemies.Add(hit.collider);
            }
        }
    }

    private void ProcessHit(Collider2D enemyCollider, int damage)
    {
        Enemy enemyComponent = enemyCollider.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.TakeDamage(damage);
        }
    }

    public IEnumerator TemporarilyDisableCollider(Collider2D platformCollider)
    {
        if (platformCollider != null)
        {
            platformCollider.enabled = false;
            yield return new WaitForSeconds(0.3f);
            platformCollider.enabled = true;
        }
    }

    public void SetCollider(Vector2 size, Vector2 offset)
    {
        PlayerCollider.size = size;
        PlayerCollider.offset = offset;
    }

    public void ResetCollider()
    {
        SetCollider(originalColliderSize, originalColliderOffset);
    }

    private IEnumerator JumpInputStopRoutine()
    {
        JumpInput = true;
        yield return new WaitForEndOfFrame();
        JumpInput = false;
    }

    private IEnumerator DashInputStopRoutine()
    {
        DashInput = true;
        yield return new WaitForEndOfFrame();
        DashInput = false;
    }

    private IEnumerator AttackInputStopRoutine()
    {
        AttackInput = true;
        yield return new WaitForEndOfFrame();
        AttackInput = false;
    }

    public void SetVelocity(float x, float y) => Rb.velocity = new Vector2(x, y);
    public void ResetGravity() => Rb.gravityScale = defaultGravityScale;
    public void SetGravity(float scale) => Rb.gravityScale = scale;
    public void SetLayer(int layer) => gameObject.layer = layer;

    public void CheckAndFlip(float xInput)
    {
        if (xInput != 0 && (IsFacingRight && xInput < 0) || (!IsFacingRight && xInput > 0))
        {
            IsFacingRight = !IsFacingRight;

            if (IsFacingRight)
            {
                visuals.localScale = originalVisualsScale;
                visuals.localPosition = originalVisualsPosition;
            }
            else
            {
                visuals.localScale = new Vector3(-originalVisualsScale.x, originalVisualsScale.y, originalVisualsScale.z);
                visuals.localPosition = new Vector3(-originalVisualsPosition.x, originalVisualsPosition.y, originalVisualsPosition.z);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(GroundCheck.position, new Vector3(GroundCheck.position.x, GroundCheck.position.y - stats.groundCheckDistance, GroundCheck.position.z));
        }

        if (Application.isPlaying && stats != null && stats.attackChain.Length > 0)
        {
            AttackData representativeAttack = stats.attackChain[0];

            Vector2 groundAttackPointOffset = representativeAttack.groundAttackPointOffset;
            Vector2 groundAttackBoxSize = representativeAttack.groundAttackBoxSize;
            Vector2 groundAttackPointPos = (Vector2)Rb.position + new Vector2(groundAttackPointOffset.x * (IsFacingRight ? 1 : -1), groundAttackPointOffset.y);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundAttackPointPos, groundAttackBoxSize);

            Vector2 airAttackPointOffset = representativeAttack.airAttackPointOffset;
            Vector2 airAttackBoxSize = representativeAttack.airAttackBoxSize;
            Vector2 airAttackPointPos = (Vector2)Rb.position + new Vector2(airAttackPointOffset.x * (IsFacingRight ? 1 : -1), airAttackPointOffset.y);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(airAttackPointPos, airAttackBoxSize);
        }
    }
}
