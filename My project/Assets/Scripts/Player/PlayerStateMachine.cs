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

        // CurrentState가 아직 설정되지 않은 경우(초기 프레임 보호)
        if (CurrentState == null)
        {
            CurrentState = newState;
            if (CurrentState != null)
            {
                CurrentState.Enter();
            }
            return;
        }

        PreviousState = CurrentState;   // 전이 직전 저장
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}