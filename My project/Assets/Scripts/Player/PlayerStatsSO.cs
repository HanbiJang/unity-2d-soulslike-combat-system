using UnityEngine;

[System.Serializable]
public class AttackData
{
    public string animationName = "attack";
    [Tooltip("이 공격 모션이 몇 초동안 나갈 건지")]
    public float attackDuration = 0.4f;
    [Tooltip("캐릭터 기준 어느 위치에 공격 판정을 만들지 정함")]
    public Vector2 attackPointOffset = new Vector2(1, 0.5f);

    [Header("지상 공격 판정")]
    public Vector2 groundAttackPointOffset = new Vector2(0.19f, 0.5f);
    [Tooltip("공격 박스 크기 (가로, 세로)")]
    public Vector2 groundAttackBoxSize = new Vector2(1f, 1f);
    [Header("공중 공격 판정")]
    public Vector2 airAttackPointOffset = new Vector2(0.19f, -0.5f);
    [Tooltip("공격 박스 크기 (가로, 세로)")]
    public Vector2 airAttackBoxSize = new Vector2(1f, 1f);

    [Header("데미지")]
    public int groundAttackDamage = 1;
    public int airAttackDamage = 1;
}

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStatsSO : ScriptableObject
{
    [Header("체력")]
    public int maxHealth = 5;

    [Header("스태미나")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f; public float staminaRegenRate = 15f; public float staminaRegenDelay = 1f;
    [Tooltip("달리기 시작에 필요한 최소 지속 시간(초)")]
    public float minRunStartDuration = 0.3f;
    [Header("스태미나 소비")]
    public float jumpStaminaCost = 10f;
    public float attackStaminaCost = 8f;
    public float airAttackStaminaCost = 10f;
    public float dashAttackStaminaCost = 6f;
    public float specialAttackStaminaCost = 20f;
    public float throwStaminaCost = 4f;
    [Header("이동 관련")]
    public float moveSpeed = 7f;
    public float runSpeed = 12f;
    public float jumpForce = 12f;
    [Tooltip("대시 중 점프 시 점프력 배수")]
    public float dashJumpMultiplier = 1.3f;

    [Header("벽 액션")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.5f; public float wallSlideSpeed = 3f; public Vector2 wallJumpForce = new Vector2(8f, 16f);
    [Header("대시")]
    public float dashSpeed = 25f;
    public float dashTime = 0.15f; public float dashCooldown = 0.3f;
    [Header("공격 관련")]
    public AttackData dashAttackData;
    public float attackCooldown = 0.5f;
    public float comboResetTime = 0.5f; public AttackData[] attackChain; public AttackData airAttackData;[Tooltip("공중 공격할 때 캐릭터 이동속도를 얼마나 줄일지 (0이면 제자리, 1이면 속도 유지)")]
    [Range(0f, 1f)]
    public float airAttackMoveSpeedMultiplier = 0.5f;

    [Header("방어 & 패링")]
    public float parryWindow = 0.15f;
    [Header("힐링")]
    public int maxHealCharges = 3; public int healAmount = 1; public float healingDuration = 1.5f;
    [Header("투척")]
    public GameObject projectilePrefab; public int maxThrowCharges = 10;
    public float throwCooldown = 1f;
    public float throwForce = 20f;[Tooltip("투척 애니메이션 전체 길이")]
    public float throwAnimationDuration = 0.5f;

    [Header("특수 공격")]
    public int maxSpecialAttackCharges = 2;
    public float specialAttackCooldown = 5f;
    public AttackData specialAttackData;

    [Header("피격 반응")]
    public float invincibilityDuration = 1f; public Vector2 knockbackForce = new Vector2(5f, 5f);
    [Header("숙이기")]
    public Vector2 crouchColliderSize = new Vector2(1f, 1.5f);
    public Vector2 crouchColliderOffset = new Vector2(0f, -0.25f);

    [Header("사다리")]
    public float climbSpeed = 5f;
    public float groundCheckDistance = 0.3f;
    [Header("레이어 설정")]
    public LayerMask groundLayer;
    public LayerMask ladderLayer;
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    public LayerMask climbingPlayerLayer;
}
