using UnityEngine;
public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!player.IsRunInput || player.StatsManager.CurrentStamina <= 0)
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