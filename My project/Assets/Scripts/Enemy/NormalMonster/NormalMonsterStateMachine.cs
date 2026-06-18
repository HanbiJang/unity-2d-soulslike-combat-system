public class NormalMonsterStateMachine
{
    public NormalMonsterState CurrentState { get; private set; }

    public void Initialize(NormalMonsterState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(NormalMonsterState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
