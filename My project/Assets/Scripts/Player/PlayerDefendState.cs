using UnityEngine;
public class PlayerDefendState : PlayerState
{
    private float entryTime;
    private bool isParrying;

    public PlayerDefendState(PlayerController player, string stateName) : base(player, stateName) { }

    public override void Enter()
    {
        base.Enter();
        entryTime = Time.time;
        isParrying = true; player.SetVelocity(0, 0);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isParrying && Time.time >= entryTime + player.stats.parryWindow)
        {
            isParrying = false;
            Debug.Log("패링 시간 종료. 이제부터 일반 방어입니다.");
        }

        if (!player.IsDefendInput)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public void HandleDamage(int damage, Transform damageSource)
    {
        if (isParrying)
        {
            Debug.Log("패링 성공!");
        }
        else
        {
            Debug.Log("방어 성공!");
        }
    }
}