public abstract class EnemyState
{
    protected EnemyController enemy;
    protected EnemyStateMachine stateMachine;
    public string stateName;

    protected EnemyState(EnemyController enemy, string stateName)
    {
        this.enemy = enemy;
        this.stateMachine = enemy.StateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        if (enemy.Anim != null)
        {
            enemy.Anim.Play(stateName);
        }
    }
    
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }

    public virtual void AnimationTrigger() { }
}



