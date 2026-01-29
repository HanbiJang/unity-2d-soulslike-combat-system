using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    private PlayerController controller;
    private SpriteRenderer sr;

    private int currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = controller.stats.maxHealth;
    }

    public void TakeDamage(int damage, Transform damageSource)
    {
        // #region agent log
        DebugLogUtil.Log("PlayerHealth.TakeDamage", "TakeDamage called", "{\"damage\":" + damage + ",\"isInvincible\":" + (controller.IsInvincible ? "true" : "false") + ",\"isDead\":" + (isDead ? "true" : "false") + ",\"state\":\"" + (controller.StateMachine?.CurrentState?.stateName ?? "null") + "\"}", "H3");
        // #endregion
        if (controller.IsInvincible || isDead)
        {
            // #region agent log
            DebugLogUtil.Log("PlayerHealth.TakeDamage", "Damage blocked", "{\"reason\":\"" + (controller.IsInvincible ? "invincible" : "dead") + "\"}", "H3");
            // #endregion
            return;
        }
        if (controller.StateMachine.CurrentState == controller.DefendState)
        {
            controller.DefendState.HandleDamage(damage, damageSource); return;
        }

        // 회복 중인 경우 - 회복 중단
        if (controller.StateMachine.CurrentState == controller.HealState)
        {
            controller.HealState.InterruptHealing();
            // 물약은 HealState에서 소모하므로 여기서는 바로 피해 처리
        }

        currentHealth -= damage;
        // #region agent log
        DebugLogUtil.Log("PlayerHealth.TakeDamage", "Damage applied", "{\"newHealth\":" + currentHealth + "}", "H3");
        // #endregion
        // Debug.Log("플레이어가 " + damage + "의 피해를 입었습니다! 현재 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        int knockbackDirection = (transform.position.x > damageSource.position.x) ? 1 : -1;

        controller.HitState.SetKnockbackDirection(knockbackDirection);
        controller.StateMachine.ChangeState(controller.HitState);

        StartCoroutine(InvincibilityCoroutine());
    }

    private void Die()
    {
        isDead = true; 
        //Debug.Log("플레이어가 사망했습니다.");

        controller.StateMachine.ChangeState(controller.DeathState);
        
        // Game Over UI 표시
        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ShowGameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, controller.stats.maxHealth);
        Debug.Log("체력이 " + amount + "만큼 회복되었습니다! 현재 체력: " + currentHealth);
    }
    // 가드 시 직접 피해 처리 (무적 시간, 넉백 등 없이)
    public void TakeDamageDirectly(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        Debug.Log("플레이어가 " + damage + "의 피해를 입었습니다! 현재 체력: " + currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public int CurrentHealth => currentHealth;

    private float lastContactDamageTime = 0f;
    private const float contactDamageInterval = 0.5f; // 접촉 데미지 간격 (초)
    /// <summary>접촉 데미지 거리. chase로 플레이어 앞에서 멈추면 이 거리 안에 안 들어오므로 접촉 데미지 최소화.</summary>
    private const float contactDamageRange = 1.5f;

    private void Update()
    {
        // 접촉 데미지 처리 (충돌이 무시되는 보스도 포함)
        if (Time.time >= lastContactDamageTime + contactDamageInterval)
        {
            CheckContactDamage();
        }
    }

    private void CheckContactDamage()
    {
        if (controller.IsInvincible || isDead) return;
        if (controller.StateMachine.CurrentState == controller.DefendState) return;

        // 플레이어 주변의 적 감지 (chase 정지 거리 attackRange~2.5 이상이어야 멈춘 자리에서도 데미지)
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            transform.position, 
            contactDamageRange,
            controller.stats.enemyLayer
        );
        // #region agent log
        DebugLogUtil.Log("PlayerHealth.CheckContactDamage", "Checking contact damage", "{\"nearbyCount\":" + nearbyEnemies.Length + ",\"enemyLayer\":" + controller.stats.enemyLayer.value + ",\"contactRange\":" + contactDamageRange + "}", "H4");
        // #endregion

        foreach (var col in nearbyEnemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 거리 체크 (실제로 접촉했는지 확인)
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                // #region agent log
                DebugLogUtil.Log("PlayerHealth.CheckContactDamage", "Enemy found", "{\"distance\":" + distance + ",\"isBoss\":" + (enemy.IsBoss ? "true" : "false") + ",\"attackDamage\":" + enemy.attackDamage + ",\"inRange\":" + (distance <= contactDamageRange ? "true" : "false") + "}", "H4");
                // #endregion
                if (distance <= contactDamageRange)
                {
                    // #region agent log
                    DebugLogUtil.Log("PlayerHealth.CheckContactDamage", "Applying contact damage", "{\"damage\":" + enemy.attackDamage + "}", "H4");
                    // #endregion
                    TakeDamage(enemy.attackDamage, enemy.transform);
                    lastContactDamageTime = Time.time;
                    break; // 한 프레임에 하나의 적에게만 데미지
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 일반 적과의 충돌 데미지 (보스는 충돌이 무시되므로 Update의 CheckContactDamage로 처리)
        if (((1 << collision.gameObject.layer) & controller.stats.enemyLayer) != 0)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            // #region agent log
            DebugLogUtil.Log("PlayerHealth.OnCollisionStay2D", "Collision detected", "{\"enemy\":" + (enemy != null ? "exists" : "null") + ",\"isBoss\":" + (enemy?.IsBoss ?? false ? "true" : "false") + ",\"layer\":" + collision.gameObject.layer + ",\"enemyLayer\":" + controller.stats.enemyLayer.value + "}", "H5");
            // #endregion

            if (enemy != null && !enemy.IsBoss) // 보스가 아닌 경우만
            {
                if (Time.time >= lastContactDamageTime + contactDamageInterval)
                {
                    // #region agent log
                    DebugLogUtil.Log("PlayerHealth.OnCollisionStay2D", "Applying collision damage", "{\"damage\":" + enemy.attackDamage + "}", "H5");
                    // #endregion
                    TakeDamage(enemy.attackDamage, collision.transform);
                    lastContactDamageTime = Time.time;
                }
            }
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        controller.IsInvincible = true;

        float endTime = Time.time + controller.stats.invincibilityDuration;
        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(0.1f);
        }

        controller.IsInvincible = false;
    }
}
