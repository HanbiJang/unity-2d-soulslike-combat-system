public class NMHurtState : NormalMonsterState
{
    private const float DURATION = 0.4f;
    private float timer;

    public NMHurtState(NormalMonsterController monster, string stateName)
        : base(monster, stateName) { }

    public override void Enter()
    {
        base.Enter();
        monster.StopMovement();
        timer = 0f;
    }

    public override void LogicUpdate()
    {
        timer += UnityEngine.Time.deltaTime;
        if (timer >= DURATION)
            stateMachine.ChangeState(monster.FlyingState);
    }
}
