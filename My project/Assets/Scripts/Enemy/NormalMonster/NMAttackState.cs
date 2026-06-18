public class NMAttackState : NormalMonsterState
{
    // 공격 애니메이션 총 길이 (초) - 애니메이션 길이에 맞게 조정
    private const float DURATION = 1.2f;
    private float timer;

    public NMAttackState(NormalMonsterController monster, string stateName)
        : base(monster, stateName) { }

    public override void Enter()
    {
        base.Enter();
        monster.StopMovement();
        monster.ConsumeAttack(); // 쿨다운 시작
        timer = 0f;
    }

    public override void LogicUpdate()
    {
        timer += UnityEngine.Time.deltaTime;
        if (timer >= DURATION)
            stateMachine.ChangeState(monster.FlyingState);
    }

    // 애니메이션 이벤트에서 호출 → 투사체 발사
    public override void AnimationTrigger()
    {
        monster.FireProjectile();
    }
}
