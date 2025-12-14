using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private float attackStartTime;
    private AttackData currentAttackData;
    private bool hasPerformedAttack; public bool IsGroundedAttack { get; private set; }
    public void SetIsGroundedAttack(bool isGrounded)
    {
        this.IsGroundedAttack = isGrounded;
    }
    public PlayerAttackState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        player.StatsManager.TryUseStamina(player.stats.attackStaminaCost);
        attackStartTime = Time.time;
        player.lastAttackTime = Time.time;
        hasPerformedAttack = false; player.IsAttackInputBuffered = false;
        currentAttackData = player.stats.attackChain[player.ComboCounter];

        if (player.Anim != null)
        {
            player.Anim.Play(currentAttackData.animationName);
        }
        player.SetVelocity(0, 0);
        
        // 무기 휘두르는 소리 재생 (타격 전)
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(SoundType.WeaponSwing);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.AttackInput)
        {
            player.IsAttackInputBuffered = true;
        }

        if (Time.time >= attackStartTime + currentAttackData.attackDuration)
        {
            if (player.IsAttackInputBuffered && player.ComboCounter < player.stats.attackChain.Length - 1 && player.StatsManager.CurrentStamina >= player.stats.attackStaminaCost)
            {
                player.ComboCounter++; player.AttackState.SetIsGroundedAttack(this.IsGroundedAttack);
                stateMachine.ChangeState(player.AttackState);
            }
            else
            {
                player.ComboCounter = 0; if (player.IsGrounded)
                {
                    stateMachine.ChangeState(player.IdleState);
                }
                else
                {
                    stateMachine.ChangeState(player.InAirState);
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        player.PerformAttack(true, currentAttackData);
    }
}
