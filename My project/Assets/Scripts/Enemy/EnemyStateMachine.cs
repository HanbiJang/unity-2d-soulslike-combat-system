public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }
    public EnemyState PreviousState { get; private set; }

    public void Initialize(EnemyState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        PreviousState = CurrentState;   // 전이 직전 저장
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}



