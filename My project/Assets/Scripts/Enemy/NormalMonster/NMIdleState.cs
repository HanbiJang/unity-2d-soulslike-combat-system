public class NMIdleState : NormalMonsterState
{
    public NMIdleState(NormalMonsterController monster, string stateName)
        : base(monster, stateName) { }

    public override void Enter()
    {
        base.Enter();
        monster.StopMovement();
    }

    public override void LogicUpdate()
    {
        if (monster.PlayerTarget == null) return;

        // 탐지 범위 안에 플레이어가 들어오면 추적 시작
        if (monster.GetDistanceToPlayer() <= monster.DetectionRange)
            stateMachine.ChangeState(monster.FlyingState);
    }
}
