public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }
    public PlayerState PreviousState { get; private set; }

    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        PreviousState = CurrentState;   // 전이 직전 저장
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}