using UnityEngine;

// 콤보 공격 한방 한방에 대한 설정값들
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
    public float staminaDrainRate = 20f; // 달릴 때 초당 스태미나 소모량
    public float staminaRegenRate = 15f; // 가만히 있을 때 초당 스태미나 회복량
    public float staminaRegenDelay = 1f; // 스태미나 쓰고 나서 몇 초 뒤에 회복 시작할지

    [Header("이동 관련")]
    public float moveSpeed = 7f;
    public float runSpeed = 12f;
    public float jumpForce = 12f;

    [Header("벽 액션")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.5f; // 벽을 감지할 거리
    public float wallSlideSpeed = 3f; // 벽 탈 때 미끄러지는 속도
    public Vector2 wallJumpForce = new Vector2(8f, 16f); // 벽 점프 시 (수평, 수직) 힘

    [Header("대시")]
    public float dashSpeed = 25f;
    public float dashTime = 0.15f; // 대시 지속 시간
    public float dashCooldown = 0.3f; // 대시 쿨타임

    [Header("공격 관련")]
    public AttackData dashAttackData;
    public float attackCooldown = 0.5f;
    public float comboResetTime = 0.5f; // 마지막 공격 후 콤보 리셋되기까지의 시간
    public AttackData[] attackChain; // 일반 공격 콤보 (1타, 2타, 3타...)
    public AttackData airAttackData; // 공중 공격
    [Tooltip("공중 공격할 때 캐릭터 이동속도를 얼마나 줄일지 (0이면 제자리, 1이면 속도 유지)")]
    [Range(0f, 1f)]
    public float airAttackMoveSpeedMultiplier = 0.5f;

    [Header("방어 & 패링")]
    public float parryWindow = 0.15f; // 방어 누르고 이 시간 안에 맞으면 패링 성공!

    [Header("힐링")]
    public int maxHealCharges = 3; // 최대 힐링 횟수
    public int healAmount = 1; // 한 번에 회복되는 체력 양
    public float healingDuration = 1.5f; // 힐링 모션 시간

    [Header("투척")]
    public GameObject projectilePrefab; // 날릴 투사체 프리팹
    public int maxThrowCharges = 10;
    public float throwCooldown = 1f;
    public float throwForce = 20f; // 투사체를 던지는 힘
    [Tooltip("투척 애니메이션 전체 길이")]
    public float throwAnimationDuration = 0.5f;

    [Header("특수 공격")]
    public int maxSpecialAttackCharges = 2;
    public float specialAttackCooldown = 5f;
    public AttackData specialAttackData;

    [Header("피격 반응")]
    public float invincibilityDuration = 1f; // 맞고 나서 무적 시간
    public Vector2 knockbackForce = new Vector2(5f, 5f); // 넉백될 때 받는 힘

    [Header("숙이기")]
    public Vector2 crouchColliderSize = new Vector2(1f, 1.5f);
    public Vector2 crouchColliderOffset = new Vector2(0f, -0.25f);

    [Header("사다리")]
    public float climbSpeed = 5f;
    public float groundCheckDistance = 0.3f; // 바닥 체크 거리

    [Header("레이어 설정")]
    public LayerMask groundLayer;
    public LayerMask ladderLayer;
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    public LayerMask climbingPlayerLayer;
}
