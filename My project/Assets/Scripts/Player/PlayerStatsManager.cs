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
        if (Time.time >= lastStaminaUseTime + controller.stats.staminaRegenDelay)
        {
            RegenStamina();
        }
    }

    public bool TryUseStamina(float amount)
    {
        if (CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            lastStaminaUseTime = Time.time;
            return true;
        }
        return false;
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