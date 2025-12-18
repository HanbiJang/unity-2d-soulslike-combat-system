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

        // 회복 중인 경우 - 회복 중단
        if (controller.StateMachine.CurrentState == controller.HealState)
        {
            controller.HealState.InterruptHealing();
            // 물약은 HealState에서 소모하므로 여기서는 바로 피해 처리
        }

        currentHealth -= damage;
        Debug.Log("�÷��̾ " + damage + "�� ���ظ� �Ծ����ϴ�! ���� ü��: " + currentHealth);

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
        isDead = true; Debug.Log("�÷��̾ ����߽��ϴ�.");

        controller.StateMachine.ChangeState(controller.DeathState);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, controller.stats.maxHealth);
        Debug.Log("ü���� " + amount + "��ŭ ȸ���߽��ϴ�! ���� ü��: " + currentHealth);
    }
    public int CurrentHealth => currentHealth;

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
