using UnityEngine;
public class PlayerRunState : PlayerGroundedState
{
    private float lastNonZeroXTime;
    public PlayerRunState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        lastNonZeroXTime = Time.time;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Mathf.Abs(player.Input.x) > 0f)
        {
            lastNonZeroXTime = Time.time;
        }
        bool directionIdleTooLong = Mathf.Abs(player.Input.x) == 0f && Time.time - lastNonZeroXTime > player.stats.runDirectionGraceTime;
        if (!player.IsRunInput || directionIdleTooLong || player.StatsManager.CurrentStamina <= 0)
        {
            stateMachine.ChangeState(player.MoveState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.StatsManager.TryUseStamina(player.stats.staminaDrainRate * Time.fixedDeltaTime);
        player.CheckAndFlip(player.Input.x);
        player.SetVelocity(player.stats.runSpeed * player.Input.x, player.Rb.velocity.y);
    }
}
