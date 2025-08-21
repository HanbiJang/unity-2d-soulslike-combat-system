public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;
    public string stateName;

    protected PlayerState(PlayerController player, string stateName)
    {
        this.player = player;
        this.stateMachine = player.StateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        if (player.Anim != null)
        {
            player.Anim.Play(stateName);
        }
    }
    public virtual void Exit() { }
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { }

    public virtual void AnimationTrigger() { }
}