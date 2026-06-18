public abstract class NormalMonsterState
{
    protected NormalMonsterController monster;
    protected NormalMonsterStateMachine stateMachine;
    public string stateName;

    protected NormalMonsterState(NormalMonsterController monster, string stateName)
    {
        this.monster = monster;
        this.stateMachine = monster.StateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        monster.Anim?.Play(stateName);
    }

    public virtual void Exit() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }

    // 애니메이션 이벤트 수신 (투사체 발사 타이밍 등)
    public virtual void AnimationTrigger() { }
}
