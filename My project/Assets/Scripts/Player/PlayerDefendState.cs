using UnityEngine;
public class PlayerDefendState : PlayerState
{
    private float entryTime;
    private bool isParrying;
    private bool hasParried;  // ?? ?? ??
    private float parrySuccessTime;  // ?? ?? ??
    private const float parryAnimationDuration = 0.5f;  // ?? ????? ?? ??

    public PlayerDefendState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        entryTime = Time.time;
        isParrying = true;
        hasParried = false;
        player.SetVelocity(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
        // ?? ??? ??? ? ?? ??
        player.IsInvincible = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // ?? ?? ? ????? ?? ?
        if (hasParried)
        {
            // ?? ?????? ??? ?? ??
            if (Time.time >= parrySuccessTime + parryAnimationDuration)
            {
                player.IsInvincible = false;
                hasParried = false;
                Debug.Log("?? ????? ??, ?? ??");
            }
            
            // ?? ????? ??? ??? ??
            return;
        }

        // ?? ?? ??
        if (isParrying && Time.time >= entryTime + player.stats.parryWindow)
        {
            isParrying = false;
            Debug.Log("?? ?? ??. ???? ?? ?????.");
        }

        // ?? ??? ??? Idle ???
        if (!player.IsDefendInput)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public void HandleDamage(int damage, Transform damageSource)
    {
        if (isParrying)
        {
            Debug.Log("?? ??!");
            
            // ?? ?? ??
            hasParried = true;
            isParrying = false;
            parrySuccessTime = Time.time;
            
            // ?? ??? ??
            player.IsInvincible = true;
            
            // DEFEND 1 ????? ??
            if (player.Anim != null)
            {
                player.Anim.Play("DEFEND 1");
                Debug.Log("?? ????? ??: DEFEND 1");
            }
            
            // ?? ?? ?? ??
            ParryEffect parryEffect = player.GetComponent<ParryEffect>();
            if (parryEffect != null)
            {
                parryEffect.PlayParryEffect();
            }
            else
            {
                Debug.LogWarning("ParryEffect ????? ?? ? ????!");
            }
            
            // ?? ?? ? ?? ??
            return;
        }
        
        // ?? ?: ??? 10% ??? + ??? 90%? ????? ??
        int healthDamage = Mathf.RoundToInt(damage * 0.1f);  // 10% ?? ??
        float staminaDamage = damage * 0.9f;  // 90% ???? ??
        
        // ?? ?? ??
        if (healthDamage > 0)
        {
            player.Health.TakeDamageDirectly(healthDamage);
        }
        
        // ???? ?? ??
        if (staminaDamage > 0)
        {
            player.StatsManager.UseStamina(staminaDamage);
        }
        
        Debug.Log($"?? ??! ??: -{healthDamage}, ????: -{staminaDamage:F1}");
    }
}