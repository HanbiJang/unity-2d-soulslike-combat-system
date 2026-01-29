using UnityEngine;
public class PlayerThrowState : PlayerState
{
    private float entryTime;
    public PlayerThrowState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter(); entryTime = Time.time;
        player.StatsManager.TryUseStamina(player.stats.throwStaminaCost);
        player.StatsManager.UseThrowCharge();
        player.SetVelocity(0, 0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Time.time >= entryTime + player.stats.throwAnimationDuration)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        if (player.stats.projectilePrefab != null)
        {
            Vector3 spawnPosition = player.transform.position + new Vector3(0, 0.5f, 0);
            GameObject projectile = Object.Instantiate(player.stats.projectilePrefab, spawnPosition, Quaternion.identity);

            Vector2 throwDirection = new Vector2(player.IsFacingRight ? 1 : -1, 0.2f).normalized;
            projectile.GetComponent<Rigidbody2D>().velocity = throwDirection * player.stats.throwForce;
        }
        else
        {
            Debug.LogWarning("PlayerStatsSO에 projectilePrefab이 설정되지 않았습니다!");
        }
    }
}
