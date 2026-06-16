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
        hasPerformedAttack = false;
        currentAttackData = player.stats.attackChain[player.ComboCounter];

        // 이번 공격을 트리거한 입력 소비 - 버퍼에 남아있으면 콤보 오작동 생김
        player.InputBuffer.Consume(BufferableInput.Attack, InputBuffer.AttackWindow);

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

        if (Time.time >= attackStartTime + currentAttackData.attackDuration)
        {
            // 윈도우를 애니메이션 전체 길이 + 여유로 잡음 - 초반에 눌러도 만료 안 됨
            float comboWindow = currentAttackData.attackDuration + 0.1f;
            if (player.InputBuffer.Consume(BufferableInput.Attack, comboWindow) && player.ComboCounter < player.stats.attackChain.Length - 1 && player.StatsManager.CurrentStamina >= player.stats.attackStaminaCost)
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
