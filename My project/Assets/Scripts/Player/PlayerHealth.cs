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
        if (controller.IsInvincible || isDead) return;
        if (controller.StateMachine.CurrentState == controller.DefendState)
        {
            controller.DefendState.HandleDamage(damage, damageSource); return;
        }

        currentHealth -= damage;
        Debug.Log("플레이어가 " + damage + "의 피해를 입었습니다! 현재 체력: " + currentHealth);

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
        isDead = true; Debug.Log("플레이어가 사망했습니다.");

        controller.StateMachine.ChangeState(controller.DeathState);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, controller.stats.maxHealth);
        Debug.Log("체력을 " + amount + "만큼 회복했습니다! 현재 체력: " + currentHealth);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & controller.stats.enemyLayer) != 0)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                TakeDamage(enemy.attackDamage, collision.transform);
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