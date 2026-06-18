public class NMFlyingState : NormalMonsterState
{
    public NMFlyingState(NormalMonsterController monster, string stateName)
        : base(monster, stateName) { }

    public override void LogicUpdate()
    {
        float dist = monster.GetDistanceToPlayer();

        // 탐지 범위 밖으로 나가면 복귀
        if (dist > monster.DetectionRange)
        {
            stateMachine.ChangeState(monster.IdleState);
            return;
        }

        // 공격 범위 안에 들어오고 쿨다운이 끝났으면 공격
        if (dist <= monster.AttackRange && monster.CanAttack)
        {
            stateMachine.ChangeState(monster.AttackState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        // 공격 범위 밖이면 계속 접근, 안이면 정지 (쿨다운 대기)
        if (monster.GetDistanceToPlayer() > monster.AttackRange)
            monster.MoveTowardPlayer();
        else
            monster.StopMovement();
    }
}
