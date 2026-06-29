using UnityEngine;
using System.Collections;

public class NormalMonsterController : Enemy
{
    [Header("AI 범위")]
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float attackRange = 5f;

    [Header("이동")]
    [SerializeField] float moveSpeed = 3f;

    [Header("공격")]
    [SerializeField] float attackCooldown = 2f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] LayerMask groundLayer;

    [Header("식별자")]
    [Tooltip("이 몬스터를 구분하는 고유 ID. 처치 여부를 씬 재진입 후에도 기억하려면 비워두지 말 것")]
    [SerializeField] string monsterId;

    [Header("아이템 드롭")]
    [SerializeField] ItemData[] dropItems;           // 이 몬스터가 드롭할 아이템 목록
    [SerializeField] GameObject itemPickupPrefab;    // 바닥에 생성될 픽업 프리팹
    [SerializeField] float dropScatterRadius = 0.5f; // 드롭 위치 랜덤 반경

    [Header("디버그")]
    [SerializeField] string currentStateName;

    // 컴포넌트
    public Animator Anim { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    private SpriteRenderer sr;

    // 상태 머신
    public NormalMonsterStateMachine StateMachine { get; private set; }
    public NMIdleState IdleState { get; private set; }
    public NMFlyingState FlyingState { get; private set; }
    public NMAttackState AttackState { get; private set; }
    public NMHurtState HurtState { get; private set; }
    public NMDeathState DeathState { get; private set; }

    // 플레이어
    public Transform PlayerTarget { get; private set; }

    // 프로퍼티
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public float MoveSpeed => moveSpeed;

    // 공격 쿨다운
    private float lastAttackTime = -999f;
    public bool CanAttack => Time.time - lastAttackTime >= attackCooldown;
    public void ConsumeAttack() => lastAttackTime = Time.time;

    protected override void Awake()
    {
        // 이미 처치한 몬스터면 다시 소환하지 않음
        if (MonsterDefeatManager.Instance != null && MonsterDefeatManager.Instance.IsDefeated(monsterId))
        {
            DestroyImmediate(gameObject);
            return;
        }

        base.Awake();
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        PlayerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;

        StateMachine = new NormalMonsterStateMachine();
        IdleState = new NMIdleState(this, "NMIdle");
        FlyingState = new NMFlyingState(this, "NMFlying");
        AttackState = new NMAttackState(this, "NMAttack");
        HurtState = new NMHurtState(this, "NMHurt");
        DeathState = new NMDeathState(this, "NMDeath");

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        currentStateName = StateMachine.CurrentState.stateName;
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }

    // 애니메이션 이벤트 → 현재 상태로 전달
    public void AnimationTrigger()
    {
        StateMachine.CurrentState.AnimationTrigger();
    }

    public float GetDistanceToPlayer()
    {
        if (PlayerTarget == null) return float.MaxValue;
        return Vector2.Distance(transform.position, PlayerTarget.position);
    }

    // 플레이어 방향으로 날아서 이동
    public void MoveTowardPlayer()
    {
        if (PlayerTarget == null) return;
        Vector2 dir = (PlayerTarget.position - transform.position).normalized;
        Rb.velocity = dir * moveSpeed;
        if (sr != null)
            sr.flipX = dir.x >= 0;
    }

    public void StopMovement()
    {
        Rb.velocity = Vector2.zero;
    }

    // 플레이어 방향으로 투사체 발사
    public void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null || PlayerTarget == null) return;

        Vector2 dir = (PlayerTarget.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile projScript = proj.GetComponent<EnemyProjectile>();
        if (projScript != null)
        {
            Collider2D[] myColliders = GetComponentsInChildren<Collider2D>();
            projScript.Launch(dir, myColliders, groundLayer);
        }
    }

    public override void TakeDamage(int damage)
    {
        // 이미 죽은 상태면 무시
        if (StateMachine == null || StateMachine.CurrentState == DeathState) return;

        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            MonsterDefeatManager.Instance?.MarkDefeated(monsterId);
            StateMachine.ChangeState(DeathState);
            return;
        }

        StateMachine.ChangeState(HurtState);
    }

    private IEnumerator FlashRed()
    {
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }

    // 죽은 자리 근처에 아이템들을 생성 (NMDeathState.Enter에서 호출)
    public void DropItems()
    {
        if (itemPickupPrefab == null) return;

        foreach (var item in dropItems)
        {
            if (item == null) continue;

            // 랜덤한 방향과 거리로 드롭 위치 계산
            Vector2 offset = Random.insideUnitCircle * dropScatterRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);

            // 픽업 오브젝트 생성 후 아이템 데이터 주입
            GameObject pickup = Instantiate(itemPickupPrefab, spawnPos, Quaternion.identity);
            ItemPickup pickupScript = pickup.GetComponent<ItemPickup>();
            if (pickupScript != null)
            {
                pickupScript.SetItem(item);
                pickupScript.SetGroundLayer(groundLayer); // 몬스터의 groundLayer 재사용
            }
        }
    }

    // 애니메이션 끝나고 Destroy하므로 base.Die()는 호출 안 함
    protected override void Die() { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
