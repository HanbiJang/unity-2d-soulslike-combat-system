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
            
            // 패링 성공 사운드
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(SoundType.ParrySuccess);
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
            Vector2 parryPos = new Vector2(player.transform.position.x, player.transform.position.y * 1.6f);
            EffectManager.Instance?.PlayParryEffect(parryPos);

            // ?? ?? ? ?? ??
            return;
        }
        
        // 일반 가드 사운드 (패링 실패)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.ParryBlock);
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