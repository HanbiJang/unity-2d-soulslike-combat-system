using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter(); player.SetVelocity(0, 0);
        player.Rb.bodyType = RigidbodyType2D.Static;
        player.PlayerCollider.enabled = false;
    }

    public override void LogicUpdate() { }
    public override void PhysicsUpdate() { }
}
