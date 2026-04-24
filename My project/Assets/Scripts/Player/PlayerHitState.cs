using UnityEngine;

public class PlayerHitState : PlayerState
{
    private float hitStartTime;
    private int knockbackDirection;

    public PlayerHitState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        hitStartTime = Time.time;

        Vector2 knockback = new Vector2(player.stats.knockbackForce.x * knockbackDirection, player.stats.knockbackForce.y);
        player.SetVelocity(knockback.x, knockback.y);
        
        // 맞을 때 음성 재생
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.PlayerHit);
        }

        EffectManager.Instance?.PlayPlayerHitEffect(player.Rb.position);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= hitStartTime + player.stats.invincibilityDuration)
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public void SetKnockbackDirection(int direction)
    {
        this.knockbackDirection = direction;
    }
}