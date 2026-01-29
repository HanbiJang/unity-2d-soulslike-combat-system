using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerStatsManager : MonoBehaviour
{
    private PlayerController controller;

    public float CurrentStamina { get; private set; }
    public int CurrentHealCharges { get; private set; }
    public int CurrentThrowCharges { get; private set; }
    public int CurrentSpecialAttackCharges { get; private set; }

    private float lastStaminaUseTime;
    private float lastThrowTime;
    private float lastSpecialAttackTime;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        CurrentStamina = controller.stats.maxStamina;
        CurrentHealCharges = controller.stats.maxHealCharges;
        CurrentThrowCharges = controller.stats.maxThrowCharges;
        CurrentSpecialAttackCharges = controller.stats.maxSpecialAttackCharges;

        lastThrowTime = -controller.stats.throwCooldown;
        lastSpecialAttackTime = -controller.stats.specialAttackCooldown;
    }

    private void Update()
    {
        // 가드 중에는 스태미너가 회복되지 않음
        bool isDefending = controller.StateMachine.CurrentState == controller.DefendState;
        
        if (!isDefending && Time.time >= lastStaminaUseTime + controller.stats.staminaRegenDelay)
        {
            RegenStamina();
        }
    }

    public bool TryUseStamina(float amount)
    {
        float toUse = Mathf.Min(amount, CurrentStamina);
        if (toUse > 0f)
        {
            CurrentStamina -= toUse;
            lastStaminaUseTime = Time.time;
            return true;
        }
        return false;
    }
    
    // 스태미나 직접 감소 (가드 시 피해용)
    public void UseStamina(float amount)
    {
        CurrentStamina -= amount;
        CurrentStamina = Mathf.Max(0f, CurrentStamina);  // 0 이하로 내려가지 않도록
        lastStaminaUseTime = Time.time;
    }

    private void RegenStamina()
    {
        if (CurrentStamina < controller.stats.maxStamina)
        {
            CurrentStamina += controller.stats.staminaRegenRate * Time.deltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, controller.stats.maxStamina);
        }
    }

    public bool CanHeal() => CurrentHealCharges > 0;
    public void UseHealCharge()
    {
        if (CanHeal())
        {
            CurrentHealCharges--;
        }
    }

    public bool CanThrow() => CurrentThrowCharges > 0 && Time.time >= lastThrowTime + controller.stats.throwCooldown;
    public void UseThrowCharge()
    {
        if (CanThrow())
        {
            CurrentThrowCharges--;
            lastThrowTime = Time.time;
        }
    }

    public bool CanUseSpecialAttack() => CurrentSpecialAttackCharges > 0 && Time.time >= lastSpecialAttackTime + controller.stats.specialAttackCooldown;
    public void UseSpecialAttackCharge()
    {
        if (CanUseSpecialAttack())
        {
            CurrentSpecialAttackCharges--;
            lastSpecialAttackTime = Time.time;
        }
    }
}
